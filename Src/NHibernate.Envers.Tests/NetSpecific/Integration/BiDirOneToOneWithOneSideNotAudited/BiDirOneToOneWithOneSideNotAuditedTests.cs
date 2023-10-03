using System;
using System.Linq;
using NHibernate.Envers.Query;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BiDirOneToOneWithOneSideNotAudited
{
	public partial class BiDirOneToOneWithOneSideNotAuditedTests : TestBase
	{
		private Guid parentId;
		private string childLatestStr;

		public BiDirOneToOneWithOneSideNotAuditedTests(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new Parent { Description = "parent 1 first description" };
			parent.Child = new Child { Str = "child of first parent", Parent = parent };

			//rev 0
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(parent);
				Session.Save(parent.Child);
				tx.Commit();
			}

			parentId = parent.Id;

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				parent.Description += "modified";
				tx.Commit();
			}

			//rev 1 (not audited child modified)
			using (var tx = Session.BeginTransaction())
			{
				parent.Child.Str += " modified";
				tx.Commit();
			}

			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				parent.Description += " modified2";
				tx.Commit();
			}

			childLatestStr = parent.Child.Str;
		}

		[Test]
		public void QueryHistoryShouldNotThrow()
		{
			Assert.DoesNotThrow(() =>
				{
					AuditReader().CreateQuery().ForRevisionsOf<Parent>().Results().ToList();
				});
		}

		[Test]
		public void QueryHistoryReturnAllRevisions()
		{
			var parentRevisions = AuditReader().CreateQuery().ForRevisionsOf<Parent>()
				.Add(AuditEntity.Id().Eq(parentId))
				.Results().ToList();

			Assert.AreEqual(3, parentRevisions.Count);
		}

		[Test]
		public void ParentsHistoricalVersionsPointsToCurrentNotAuditedChild()
		{
			var parentRevisions = AuditReader().CreateQuery().ForRevisionsOf<Parent>()
				.Add(AuditEntity.Id().Eq(parentId))
				.Results().ToList();

			foreach (var revision in parentRevisions)
			{
				Assert.AreEqual(childLatestStr, revision.Child.Str);
			}
		}
	}
}
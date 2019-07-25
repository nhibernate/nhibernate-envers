using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Tree
{
	public class TreeTest : TestBase
	{
		private long parentId;
		private long childId;

		public TreeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings => new[] {"NetSpecific.Integration.Tree.Mapping.hbm.xml"};

		protected override void Initialize()
		{
			var parent = new TreeEntity {Name = "Parent"};

			using (var tx = Session.BeginTransaction())
			{
				parentId = (long) Session.Save(parent);
				tx.Commit();
			}

			var child = new TreeEntity
			{
				Name = "Child",
				Parent = new TreeEntity {Id = parentId}
			};

			using (var tx = Session.BeginTransaction())
			{
				childId = (long) Session.Save(child);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] {1}, AuditReader().GetRevisions(typeof(TreeEntity), parentId));
			CollectionAssert.AreEquivalent(new[] {2}, AuditReader().GetRevisions(typeof(TreeEntity), childId));
		}

		[Test]
		public void VerifyHistory()
		{
			var rev1 = AuditReader().Find<TreeEntity>(parentId, 1);
			var rev2 = AuditReader().Find<TreeEntity>(parentId, 2);

			Assert.IsNotNull(rev1.Name);
			Assert.IsNotNull(rev2.Name);
		}
	}
}
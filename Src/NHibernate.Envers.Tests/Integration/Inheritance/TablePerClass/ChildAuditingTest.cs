using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.TablePerClass
{
	public partial class ChildAuditingTest : TestBase
	{
		private int id1;

		public ChildAuditingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			id1 = 111;
			var ce = new ChildEntity { Id = id1, Data = "x", Number = 1 };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ce);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ce.Data = "y";
				ce.Number = 2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ChildEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfChild()
		{
			var ver1 = new ChildEntity { Id = id1, Data = "x", Number = 1 };
			var ver2 = new ChildEntity { Id = id1, Data = "y", Number = 2 };

			Assert.AreEqual(ver1, AuditReader().Find<ChildEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<ChildEntity>(id1, 2));
		}

		[Test]
		public void VerifyPolymorphicQuery()
		{
			var childVersion1 = new ChildEntity { Id = id1, Data = "x", Number = 1 };
			Assert.AreEqual(childVersion1, AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(ChildEntity), 1).GetSingleResult());
			Assert.AreEqual(childVersion1, AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(ParentEntity), 1).GetSingleResult());
		}
	}
}
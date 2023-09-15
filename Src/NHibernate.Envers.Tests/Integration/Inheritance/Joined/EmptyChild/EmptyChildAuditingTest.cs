using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.EmptyChild
{
	public partial class EmptyChildAuditingTest : TestBase
	{
		private int id1;

		public EmptyChildAuditingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			id1 = 1;
			var pe = new EmptyChildEntity {Id = id1, Data = "x"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(pe);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				pe.Data = "y";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(EmptyChildEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfChildId1()
		{
			var ver1 = new EmptyChildEntity { Id = id1, Data = "x" };
			var ver2 = new EmptyChildEntity { Id = id1, Data = "y" };

			Assert.AreEqual(ver1, AuditReader().Find<EmptyChildEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<EmptyChildEntity>(id1, 2));
		}

		[Test]
		public void VerifyPolymorphicQuery()
		{
			var childVer1 = new EmptyChildEntity { Id = id1, Data = "x" };

			Assert.AreEqual(childVer1, AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(EmptyChildEntity), 1).GetSingleResult());
			Assert.AreEqual(childVer1, AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(ParentEntity), 1).GetSingleResult());
		}
	}
}
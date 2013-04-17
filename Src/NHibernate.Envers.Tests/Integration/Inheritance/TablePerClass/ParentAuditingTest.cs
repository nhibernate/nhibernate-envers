using NHibernate.Dialect;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.TablePerClass
{
	public class ParentAuditingTest : TestBase
	{
		private int id1;

		public ParentAuditingTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			if (StrategyType == typeof(ValidityAuditStrategy) && !(Dialect is MsSql2000Dialect))
				Assert.Ignore("Need a fix in NH Core first - see https://nhibernate.jira.com/browse/NH-3074");
			id1 = 1;
			var ce = new ParentEntity { Id = id1, Data = "x" };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ce);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ce.Data = "y";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ParentEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfChild()
		{
			Assert.IsNull(AuditReader().Find<ChildEntity>(id1, 1));
			Assert.IsNull(AuditReader().Find<ChildEntity>(id1, 2));
		}

		[Test]
		public void VerifyHistoryOfParent()
		{
			var ver1 = new ParentEntity { Id = id1, Data = "x" };
			var ver2 = new ParentEntity { Id = id1, Data = "y" };

			Assert.AreEqual(ver1, AuditReader().Find<ParentEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<ParentEntity>(id1, 2));
		}

		[Test]
		public void VerifyPolymorphicQuery()
		{
			var childVersion1 = new ParentEntity { Id = id1, Data = "x" };
			Assert.AreEqual(childVersion1, AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(ParentEntity), 1).GetSingleResult());
			CollectionAssert.IsEmpty(AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(ChildEntity), 1).GetResultList());
		}
	}
}
using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Single.Relation
{
	public partial class PolymorphicCollectionTest : TestBase
	{
		private int ed_id1;
		private int c_id;
		private int p_id;

		public PolymorphicCollectionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed_id1 = 1;
			p_id = 10;
			c_id = 100;
			var re = new ReferencedToParentEntity {Id = ed_id1};
			var pie = new ParentIngEntity {Id = p_id, Data = "x", Referenced = re};
			var cie = new ChildNotIngEntity {Id = c_id, Data = "y", Number = 1, Referenced = re};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(re);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(pie);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(cie);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(ReferencedToParentEntity), ed_id1));
			CollectionAssert.AreEquivalent(new[] { 2 }, AuditReader().GetRevisions(typeof(ParentIngEntity), p_id));
			CollectionAssert.AreEquivalent(new[] { 3 }, AuditReader().GetRevisions(typeof(ChildNotIngEntity), c_id));
		}

		[Test]
		public void VerifyHistoryOfReferencedCollection()
		{
			CollectionAssert.IsEmpty(AuditReader().Find<ReferencedToParentEntity>(ed_id1, 1).Referencing);
			CollectionAssert.AreEquivalent(new[] { new ParentIngEntity { Id = p_id, Data = "x" } },
										AuditReader().Find<ReferencedToParentEntity>(ed_id1, 2).Referencing);
			CollectionAssert.AreEquivalent(new[] { new ParentIngEntity { Id = p_id, Data = "x" }, new ChildNotIngEntity{ Id = c_id, Data = "y", Number = 1} },
										AuditReader().Find<ReferencedToParentEntity>(ed_id1, 3).Referencing);
		}
	}
}
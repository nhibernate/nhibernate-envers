using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	public partial class BasicCollectionTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ing1_id;
		private int ing2_id;

		public BasicCollectionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed1_id = 37;
			ed2_id = 123;
			ing1_id = 3;
			ing2_id = 55;

			var ed1 = new CollectionRefEdEntity {Id = ed1_id, Data = "data_ed_1"};
			var ed2 = new CollectionRefEdEntity { Id = ed2_id, Data = "data_ed_2" };
			var ing1 = new CollectionRefIngEntity { Id = ing1_id, Data = "data_ing_1", Reference = ed1 };
			var ing2 = new CollectionRefIngEntity { Id = ing2_id, Data = "data_ing_2", Reference = ed1 };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ing1);
				Session.Save(ing2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed2;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing2.Reference = ed2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(CollectionRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(CollectionRefEdEntity), ed2_id));

			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(CollectionRefIngEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(CollectionRefIngEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<CollectionRefIngEntity>(ing1_id);
			var ing2 = Session.Get<CollectionRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<CollectionRefEdEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<CollectionRefEdEntity>(ed1_id, 2);
			var rev3 = AuditReader().Find<CollectionRefEdEntity>(ed1_id, 3);

			CollectionAssert.AreEquivalent(new[] {ing1, ing2}, rev1.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev2.Reffering);
			CollectionAssert.IsEmpty(rev3.Reffering);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<CollectionRefIngEntity>(ing1_id);
			var ing2 = Session.Get<CollectionRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<CollectionRefEdEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<CollectionRefEdEntity>(ed2_id, 2);
			var rev3 = AuditReader().Find<CollectionRefEdEntity>(ed2_id, 3);

			CollectionAssert.IsEmpty(rev1.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing1 }, rev2.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing2, ing1 }, rev3.Reffering);
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1 = Session.Get<CollectionRefEdEntity>(ed1_id);
			var ed2 = Session.Get<CollectionRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<CollectionRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<CollectionRefIngEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<CollectionRefIngEntity>(ing1_id, 3);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed2, rev2.Reference);
			Assert.AreEqual(ed2, rev3.Reference);
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed1 = Session.Get<CollectionRefEdEntity>(ed1_id);
			var ed2 = Session.Get<CollectionRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<CollectionRefIngEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<CollectionRefIngEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<CollectionRefIngEntity>(ing2_id, 3);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.AreEqual(ed2, rev3.Reference);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"Entities.OneToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml"};
			}
		}
	}
}
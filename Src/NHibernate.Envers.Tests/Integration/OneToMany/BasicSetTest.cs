using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	public partial class BasicSetTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ing1_id;
		private int ing2_id;

		public BasicSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed1_id = 45;
			ed2_id = 123;
			ing1_id = 1000;
			ing2_id = 2000;
			var ed1 = new SetRefEdEntity {Id = ed1_id, Data = "data_ed_1"};
			var ed2 = new SetRefEdEntity {Id = ed2_id, Data = "data_ed_2"};
			var ing1 = new SetRefIngEntity { Id = ing1_id, Data = "data_ing_1" };
			var ing2 = new SetRefIngEntity { Id = ing2_id, Data = "data_ing_2" };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed1;
				ing2.Reference = ed1;
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
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(SetRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, AuditReader().GetRevisions(typeof(SetRefEdEntity), ed2_id));

			CollectionAssert.AreEquivalent(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(SetRefIngEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 2, 4 }, AuditReader().GetRevisions(typeof(SetRefIngEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<SetRefIngEntity>(ing1_id);
			var ing2 = Session.Get<SetRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<SetRefEdEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<SetRefEdEntity>(ed1_id, 2);
			var rev3 = AuditReader().Find<SetRefEdEntity>(ed1_id, 3);
			var rev4 = AuditReader().Find<SetRefEdEntity>(ed1_id, 4);

			CollectionAssert.IsEmpty(rev1.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev2.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev3.Reffering);
			CollectionAssert.IsEmpty(rev4.Reffering);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<SetRefIngEntity>(ing1_id);
			var ing2 = Session.Get<SetRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<SetRefEdEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<SetRefEdEntity>(ed2_id, 2);
			var rev3 = AuditReader().Find<SetRefEdEntity>(ed2_id, 3);
			var rev4 = AuditReader().Find<SetRefEdEntity>(ed2_id, 4);

			CollectionAssert.IsEmpty(rev1.Reffering);
			CollectionAssert.IsEmpty(rev2.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing1 }, rev3.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev4.Reffering);
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1 = Session.Get<SetRefEdEntity>(ed1_id);
			var ed2 = Session.Get<SetRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<SetRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<SetRefIngEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<SetRefIngEntity>(ing1_id, 3);
			var rev4 = AuditReader().Find<SetRefIngEntity>(ing1_id, 4);

			Assert.IsNull(rev1);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.AreEqual(ed2, rev3.Reference);
			Assert.AreEqual(ed2, rev4.Reference);
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed1 = Session.Get<SetRefEdEntity>(ed1_id);
			var ed2 = Session.Get<SetRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<SetRefIngEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<SetRefIngEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<SetRefIngEntity>(ing2_id, 3);
			var rev4 = AuditReader().Find<SetRefIngEntity>(ing2_id, 4);

			Assert.IsNull(rev1);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.AreEqual(ed1, rev3.Reference);
			Assert.AreEqual(ed2, rev4.Reference);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	public partial class BasicSetWithNullsDeleteTest : TestBase
	{
		private const int ed1_id =123;
		private const int ed2_id =54;
		private const int ing1_id =66;
		private const int ing2_id =87;
		private const int ing3_id =100;
		private const int ing4_id =200;

		public BasicSetWithNullsDeleteTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1 = new SetRefEdEntity {Id = ed1_id, Data = "data_ed_1"};
			var ed2 = new SetRefEdEntity {Id = ed2_id, Data = "data_ed_2"};
			var ing1 = new SetRefIngEntity {Id = ing1_id, Data = "data_ing_1", Reference = ed1};
			var ing2 = new SetRefIngEntity {Id = ing2_id, Data = "data_ing_2", Reference = ed1};
			var ing3 = new SetRefIngEntity {Id = ing3_id, Data = "data_ing_3", Reference = ed1};
			var ing4 = new SetRefIngEntity {Id = ing4_id, Data = "data_ing_4", Reference = ed1};

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ing1);
				Session.Save(ing2);
				Session.Save(ing3);
				Session.Save(ing4);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = null;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(ing2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing3.Reference = ed2;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(ed1);
				ing4.Reference = null;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, AuditReader().GetRevisions(typeof(SetRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 4 }, AuditReader().GetRevisions(typeof(SetRefEdEntity), ed2_id));

			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(SetRefIngEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(SetRefIngEntity), ing2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 4 }, AuditReader().GetRevisions(typeof(SetRefIngEntity), ing3_id));
			CollectionAssert.AreEquivalent(new[] { 1, 5 }, AuditReader().GetRevisions(typeof(SetRefIngEntity), ing4_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<SetRefIngEntity>(ing1_id);
			var ing2 = new SetRefIngEntity {Id = ing2_id, Data = "data_ing_2"};
			var ing3 = Session.Get<SetRefIngEntity>(ing3_id);
			var ing4 = Session.Get<SetRefIngEntity>(ing4_id);

			var rev1 = AuditReader().Find<SetRefEdEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<SetRefEdEntity>(ed1_id, 2);
			var rev3 = AuditReader().Find<SetRefEdEntity>(ed1_id, 3);
			var rev4 = AuditReader().Find<SetRefEdEntity>(ed1_id, 4);
			var rev5 = AuditReader().Find<SetRefEdEntity>(ed1_id, 5);

			CollectionAssert.AreEquivalent(new[] { ing1, ing2, ing3, ing4 }, rev1.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing2, ing3, ing4 }, rev2.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing3, ing4 }, rev3.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing4 }, rev4.Reffering);
			Assert.IsNull(rev5);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing3 = Session.Get<SetRefIngEntity>(ing3_id);

			var rev1 = AuditReader().Find<SetRefEdEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<SetRefEdEntity>(ed2_id, 2);
			var rev3 = AuditReader().Find<SetRefEdEntity>(ed2_id, 3);
			var rev4 = AuditReader().Find<SetRefEdEntity>(ed2_id, 4);
			var rev5 = AuditReader().Find<SetRefEdEntity>(ed2_id, 5);

			CollectionAssert.IsEmpty(rev1.Reffering);
			CollectionAssert.IsEmpty(rev2.Reffering);
			CollectionAssert.IsEmpty(rev3.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing3 }, rev4.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing3 }, rev5.Reffering);
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1 = new SetRefEdEntity {Id = ed1_id, Data = "data_ed_1"};

			var rev1 = AuditReader().Find<SetRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<SetRefIngEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<SetRefIngEntity>(ing1_id, 3);
			var rev4 = AuditReader().Find<SetRefIngEntity>(ing1_id, 4);
			var rev5= AuditReader().Find<SetRefIngEntity>(ing1_id, 5);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.IsNull(rev2.Reference);
			Assert.IsNull(rev3.Reference);
			Assert.IsNull(rev4.Reference);
			Assert.IsNull(rev5.Reference);
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed1 = new SetRefEdEntity { Id = ed1_id, Data = "data_ed_1" };

			var rev1 = AuditReader().Find<SetRefIngEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<SetRefIngEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<SetRefIngEntity>(ing2_id, 3);
			var rev4 = AuditReader().Find<SetRefIngEntity>(ing2_id, 4);
			var rev5 = AuditReader().Find<SetRefIngEntity>(ing2_id, 5);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.IsNull(rev3);
			Assert.IsNull(rev4);
			Assert.IsNull(rev5);
		}

		[Test]
		public void VerifyHistoryOfIng3()
		{
			var ed1 = new SetRefEdEntity { Id = ed1_id, Data = "data_ed_1" };
			var ed2 = new SetRefEdEntity { Id = ed2_id, Data = "data_ed_2" };

			var rev1 = AuditReader().Find<SetRefIngEntity>(ing3_id, 1);
			var rev2 = AuditReader().Find<SetRefIngEntity>(ing3_id, 2);
			var rev3 = AuditReader().Find<SetRefIngEntity>(ing3_id, 3);
			var rev4 = AuditReader().Find<SetRefIngEntity>(ing3_id, 4);
			var rev5 = AuditReader().Find<SetRefIngEntity>(ing3_id, 5);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.AreEqual(ed1, rev3.Reference);
			Assert.AreEqual(ed2, rev4.Reference);
			Assert.AreEqual(ed2, rev5.Reference);
		}

		[Test]
		public void VerifyHistoryOfIng4()
		{
			var ed1 = new SetRefEdEntity { Id = ed1_id, Data = "data_ed_1" };

			var rev1 = AuditReader().Find<SetRefIngEntity>(ing4_id, 1);
			var rev2 = AuditReader().Find<SetRefIngEntity>(ing4_id, 2);
			var rev3 = AuditReader().Find<SetRefIngEntity>(ing4_id, 3);
			var rev4 = AuditReader().Find<SetRefIngEntity>(ing4_id, 4);
			var rev5 = AuditReader().Find<SetRefIngEntity>(ing4_id, 5);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.AreEqual(ed1, rev3.Reference);
			Assert.AreEqual(ed1, rev4.Reference);
			Assert.IsNull(rev5.Reference);
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
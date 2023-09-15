using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.Ids
{
	public partial class EmbIdBidirectionalTest : TestBase
	{
		private EmbId ed1_id;
		private EmbId ed2_id;
		private EmbId ing1_id;

		public EmbIdBidirectionalTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed1_id = new EmbId {X = 1, Y = 2};
			ed2_id = new EmbId {X = 3, Y = 4};
			ing1_id = new EmbId { X = 5, Y = 6 };
			var ed1 = new BiEmbIdRefEdEntity {Id = ed1_id, Data = "data_ed_1"};
			var ed2 = new BiEmbIdRefEdEntity { Id = ed2_id, Data = "data_ed_2" };
			var ing1 = new BiEmbIdRefIngEntity {Id = ing1_id, Data = "data_ing_1"};
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed1;
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ing1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BiEmbIdRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BiEmbIdRefEdEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BiEmbIdRefIngEntity), ing1_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<BiEmbIdRefIngEntity>(ing1_id);

			var rev1 = AuditReader().Find<BiEmbIdRefEdEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<BiEmbIdRefEdEntity>(ed1_id, 2);

			Assert.AreEqual(ing1, rev1.Referencing);
			Assert.IsNull(rev2.Referencing);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<BiEmbIdRefIngEntity>(ing1_id);

			var rev1 = AuditReader().Find<BiEmbIdRefEdEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<BiEmbIdRefEdEntity>(ed2_id, 2);

			Assert.IsNull(rev1.Referencing);
			Assert.AreEqual(ing1, rev2.Referencing);
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1 = Session.Get<BiEmbIdRefEdEntity>(ed1_id);
			var ed2 = Session.Get<BiEmbIdRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<BiEmbIdRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<BiEmbIdRefIngEntity>(ing1_id, 2);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed2, rev2.Reference);
		}
	}
}
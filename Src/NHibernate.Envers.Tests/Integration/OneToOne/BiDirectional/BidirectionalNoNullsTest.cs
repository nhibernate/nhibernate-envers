using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	public class BidirectionalNoNullsTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ing1_id;
		private int ing2_id;

		public BidirectionalNoNullsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1 = new BiRefEdEntity { Id = 1, Data = "data_ed_1" };
			var ed2 = new BiRefEdEntity { Id = 2, Data = "data_ed_2" };
			var ing1 = new BiRefIngEntity { Id = 3, Data = "data_ing_1"};
			var ing2 = new BiRefIngEntity { Id = 4, Data = "data_ing_2"};

			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed1;
				ing2.Reference = ed2;

				ed1_id = (int)Session.Save(ed1);
				ed2_id = (int)Session.Save(ed2);
				ing1_id = (int)Session.Save(ing1);
				ing2_id = (int)Session.Save(ing2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed2;
				ing2.Reference = ed1;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BiRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BiRefEdEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BiRefIngEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BiRefIngEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<BiRefIngEntity>(ing1_id);
			var ing2 = Session.Get<BiRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<BiRefEdEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<BiRefEdEntity>(ed1_id, 2);

			Assert.AreEqual(ing1, rev1.Referencing);
			Assert.AreEqual(ing2, rev2.Referencing);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<BiRefIngEntity>(ing1_id);
			var ing2 = Session.Get<BiRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<BiRefEdEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<BiRefEdEntity>(ed2_id, 2);

			Assert.AreEqual(ing2, rev1.Referencing);
			Assert.AreEqual(ing1, rev2.Referencing);
		}
	}
}
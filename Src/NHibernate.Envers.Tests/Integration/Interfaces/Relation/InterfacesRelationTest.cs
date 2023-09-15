using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Relation
{
	public partial class InterfacesRelationTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ing1_id;

		public InterfacesRelationTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed1_id = 56;
			ed2_id = 132;
			ing1_id = 3;
			var ed1 = new SetRefEdEntity { Id = ed1_id, Data = "data_ed_1" };
			var ed2 = new SetRefEdEntity { Id = ed2_id, Data = "data_ed_2" };
			var ing1 = new SetRefIngEntity {Id = 3, Data = "data_ing_1"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed1;
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
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(SetRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(SetRefEdEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(SetRefIngEntity), ing1_id));
		}

		[Test]
		public void VerifyHistoryOfEdIng1()
		{
			var ed1 = Session.Get<SetRefEdEntity>(ed1_id);
			var ed2 = Session.Get<SetRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<SetRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<SetRefIngEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<SetRefIngEntity>(ing1_id, 3);

			Assert.IsNull(rev1);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.AreEqual(ed2, rev3.Reference);
		}
	}
}
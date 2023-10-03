using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToOne.UniDirectional
{
	public partial class UnidirectionalTest : TestBase
	{
		private const int ed1_id = 1;
		private const int ed2_id = 2;
		private const int ed3_id = 3;
		private const int ed4_id = 4;
		private const int ing1_id = 5;
		private const int ing2_id = 6;

		public UnidirectionalTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1 = new UniRefEdEntity {Id = ed1_id, Data = "data_ed_1"};
			var ed2 = new UniRefEdEntity {Id = ed2_id, Data = "data_ed_2"};
			var ed3 = new UniRefEdEntity {Id = ed3_id, Data = "data_ed_3"};
			var ed4 = new UniRefEdEntity {Id = ed4_id, Data = "data_ed_4"};
			var ing1 = new UniRefIngEntity { Id = ing1_id, Data = "data_ing_1", Reference = ed1};
			var ing2 = new UniRefIngEntity { Id = ing2_id, Data = "data_ing_2", Reference = ed3 };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ed3);
				Session.Save(ed4);
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
				ing2.Reference = ed4;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(UniRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(UniRefEdEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(UniRefEdEntity), ed3_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(UniRefEdEntity), ed4_id));

			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(UniRefIngEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(UniRefIngEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1 = Session.Get<UniRefEdEntity>(ed1_id);
			var ed2 = Session.Get<UniRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<UniRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<UniRefIngEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<UniRefIngEntity>(ing1_id, 3);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed2, rev2.Reference);
			Assert.AreEqual(ed2, rev3.Reference);
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed3 = Session.Get<UniRefEdEntity>(ed3_id);
			var ed4 = Session.Get<UniRefEdEntity>(ed4_id);

			var rev1 = AuditReader().Find<UniRefIngEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<UniRefIngEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<UniRefIngEntity>(ing2_id, 3);

			Assert.AreEqual(ed3, rev1.Reference);
			Assert.AreEqual(ed3, rev2.Reference);
			Assert.AreEqual(ed4, rev3.Reference);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Ids.Mapping.hbm.xml", "Integration.OneToOne.UniDirectional.Mapping.hbm.xml" };
			}
		}
	}
}
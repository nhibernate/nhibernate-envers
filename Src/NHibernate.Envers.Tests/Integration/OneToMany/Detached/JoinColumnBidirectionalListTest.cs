using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class JoinColumnBidirectionalListTest :TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ing1_id;
		private int ing2_id;

		public JoinColumnBidirectionalListTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			// Revision 1 (ing1: ed1, ing2: ed2)
			using (var tx = Session.BeginTransaction())
			{
				var ed1 = new ListJoinColumnBidirectionalRefEdEntity { Data = "ed1" };
				var ed2 = new ListJoinColumnBidirectionalRefEdEntity { Data = "ed2" };
				var ing1 = new ListJoinColumnBidirectionalRefIngEntity { Data = "coll1", References = new List<ListJoinColumnBidirectionalRefEdEntity> { ed1 } };
				var ing2 = new ListJoinColumnBidirectionalRefIngEntity { Data = "coll1", References = new List<ListJoinColumnBidirectionalRefEdEntity> { ed2 } };

				ed1_id = (int) Session.Save(ed1);
				ed2_id = (int)Session.Save(ed2);
				ing1_id = (int)Session.Save(ing1);
				ing2_id = (int)Session.Save(ing2);
				tx.Commit();
				Session.Clear();
			}
			// Revision 2 (ing1: ed1, ed2)
			using (var tx = Session.BeginTransaction())
			{
				var ing1 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing1_id);
				var ing2 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing2_id);
				var ed2 = Session.Get<ListJoinColumnBidirectionalRefEdEntity>(ed2_id);
				ing2.References.Remove(ed2);
				ing1.References.Add(ed2);
				tx.Commit();
				Session.Clear();
			}
			// No revision - no changes
			using (var tx = Session.BeginTransaction())
			{
				var ing2 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing2_id);
				var ed1 = Session.Get<ListJoinColumnBidirectionalRefEdEntity>(ed1_id);
				ed1.Data = "ed1 bis";
				// Shouldn't get written
				ed1.Owner = ing2;
				tx.Commit();
				Session.Clear();
			}
			// Revision 4 (ing2: ed1, ed2)
			using (var tx = Session.BeginTransaction())
			{
				var ing1 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing1_id);
				var ing2 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing2_id);
				var ed1 = Session.Get<ListJoinColumnBidirectionalRefEdEntity>(ed1_id);
				var ed2 = Session.Get<ListJoinColumnBidirectionalRefEdEntity>(ed2_id);
				ing1.References.Clear();
				ing2.References.Add(ed1);
				ing2.References.Add(ed2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof(ListJoinColumnBidirectionalRefIngEntity), ing1_id)
				.Should().Have.SameSequenceAs(1, 2, 4);
			AuditReader().GetRevisions(typeof(ListJoinColumnBidirectionalRefIngEntity), ing2_id)
				.Should().Have.SameSequenceAs(1, 2, 4);
			AuditReader().GetRevisions(typeof(ListJoinColumnBidirectionalRefEdEntity), ed1_id)
				.Should().Have.SameSequenceAs(1, 3, 4);
			AuditReader().GetRevisions(typeof(ListJoinColumnBidirectionalRefEdEntity), ed2_id)
				.Should().Have.SameSequenceAs(1, 2, 4);
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1_fromRev1 = new ListJoinColumnBidirectionalRefEdEntity {Id = ed1_id, Data = "ed1"};
			var ed1_fromRev3 = new ListJoinColumnBidirectionalRefEdEntity { Id = ed1_id, Data = "ed1 bis" };
			var ed2 = Session.Get<ListJoinColumnBidirectionalRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<ListJoinColumnBidirectionalRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<ListJoinColumnBidirectionalRefIngEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<ListJoinColumnBidirectionalRefIngEntity>(ing1_id, 3);
			var rev4 = AuditReader().Find<ListJoinColumnBidirectionalRefIngEntity>(ing1_id, 4);

			rev1.References.Should().Have.SameValuesAs(ed1_fromRev1);
			rev2.References.Should().Have.SameValuesAs(ed1_fromRev1, ed2);
			rev3.References.Should().Have.SameValuesAs(ed1_fromRev3, ed2);
			rev4.References.Should().Be.Empty();
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed1 = Session.Get<ListJoinColumnBidirectionalRefEdEntity>(ed1_id);
			var ed2 = Session.Get<ListJoinColumnBidirectionalRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<ListJoinColumnBidirectionalRefIngEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<ListJoinColumnBidirectionalRefIngEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<ListJoinColumnBidirectionalRefIngEntity>(ing2_id, 3);
			var rev4 = AuditReader().Find<ListJoinColumnBidirectionalRefIngEntity>(ing2_id, 4);

			rev1.References.Should().Have.SameValuesAs(ed2);
			rev2.References.Should().Be.Empty();
			rev3.References.Should().Be.Empty();
			rev4.References.Should().Have.SameValuesAs(ed1, ed2);
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing1_id);
			var ing2 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<ListJoinColumnBidirectionalRefEdEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<ListJoinColumnBidirectionalRefEdEntity>(ed1_id, 2);
			var rev3 = AuditReader().Find<ListJoinColumnBidirectionalRefEdEntity>(ed1_id, 3);
			var rev4 = AuditReader().Find<ListJoinColumnBidirectionalRefEdEntity>(ed1_id, 4);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing2, rev4.Owner);

			Assert.AreEqual("ed1", rev1.Data);
			Assert.AreEqual("ed1", rev2.Data);
			Assert.AreEqual("ed1 bis", rev3.Data);
			Assert.AreEqual("ed1 bis", rev4.Data);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing1_id);
			var ing2 = Session.Get<ListJoinColumnBidirectionalRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<ListJoinColumnBidirectionalRefEdEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<ListJoinColumnBidirectionalRefEdEntity>(ed2_id, 2);
			var rev3 = AuditReader().Find<ListJoinColumnBidirectionalRefEdEntity>(ed2_id, 3);
			var rev4 = AuditReader().Find<ListJoinColumnBidirectionalRefEdEntity>(ed2_id, 4);

			Assert.AreEqual(ing2, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing2, rev4.Owner);

			Assert.AreEqual("ed2", rev1.Data);
			Assert.AreEqual("ed2", rev2.Data);
			Assert.AreEqual("ed2", rev3.Data);
			Assert.AreEqual("ed2", rev4.Data);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.OneToMany.Detached.Mapping.hbm.xml" };
			}
		}
	}
}
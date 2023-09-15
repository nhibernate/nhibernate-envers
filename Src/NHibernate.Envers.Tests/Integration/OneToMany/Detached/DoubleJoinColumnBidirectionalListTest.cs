using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class DoubleJoinColumnBidirectionalListTest : TestBase
	{
		private int ed1_1_id;
		private int ed2_1_id;
		private int ed1_2_id;
		private int ed2_2_id;
		private int ing1_id;
		private int ing2_id;

		public DoubleJoinColumnBidirectionalListTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			// Revision 1 (ing1: ed1_1, ed2_1, ing2: ed1_2, ed2_2)
			using (var tx = Session.BeginTransaction())
			{
				var ed1_1 = new DoubleListJoinColumnBidirectionalRefEdEntity1 { Data = "ed1_1" };
				var ed1_2 = new DoubleListJoinColumnBidirectionalRefEdEntity1 { Data = "ed1_2" };
				var ed2_1 = new DoubleListJoinColumnBidirectionalRefEdEntity2 { Data = "ed2_1" };
				var ed2_2 = new DoubleListJoinColumnBidirectionalRefEdEntity2 { Data = "ed2_2" };
				var ing1 = new DoubleListJoinColumnBidirectionalRefIngEntity { Data = "coll1" };
				var ing2 = new DoubleListJoinColumnBidirectionalRefIngEntity { Data = "coll2" };

				ing1.References1.Add(ed1_1);
				ing1.References2.Add(ed2_1);
				ing2.References1.Add(ed1_2);
				ing2.References2.Add(ed2_2);

				ed1_1_id = (int) Session.Save(ed1_1);
				ed1_2_id = (int) Session.Save(ed1_2);
				ed2_1_id = (int) Session.Save(ed2_1);
				ed2_2_id = (int) Session.Save(ed2_2);
				ing1_id = (int) Session.Save(ing1);
				ing2_id = (int) Session.Save(ing2);
				tx.Commit();
			}
			// Revision 2 (ing1: ed1_1, ed1_2, ed2_1, ed2_2)
			using (var tx = Session.BeginTransaction())
			{
				var ing1 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id);
				var ing2 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id);
				var ed1_2 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_2_id);
				var ed2_2 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_2_id);

				ing2.References1.Clear();
				ing2.References2.Clear();
				ing1.References1.Add(ed1_2);
				ing1.References2.Add(ed2_2);
				tx.Commit();
				Session.Clear();
			}
			// Revision 3 (ing1: ed1_1, ed1_2, ed2_1, ed2_2)
			using (var tx = Session.BeginTransaction())
			{
				var ed1_1 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_1_id);
				var ed2_2 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_2_id);

				ed1_1.Data = "ed1_1 bis";
				ed2_2.Data = "ed2_2 bis";
				tx.Commit();
				Session.Clear();
			}
			// Revision 4 (ing1: ed2_2, ing2: ed2_1, ed1_1, ed1_2)
			using (var tx = Session.BeginTransaction())
			{
				var ing1 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id);
				var ing2 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id);
				var ed1_1 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_1_id);
				var ed1_2 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_2_id);
				var ed2_1 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_1_id);

				ing1.References1.Clear();
				ing2.References1.Add(ed1_1);
				ing2.References1.Add(ed1_2);

				ing1.References2.Remove(ed2_1);
				ing2.References2.Add(ed2_1);
				tx.Commit();
				Session.Clear();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, AuditReader().GetRevisions(typeof(DoubleListJoinColumnBidirectionalRefEdEntity1), ed1_1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(DoubleListJoinColumnBidirectionalRefEdEntity1), ed1_2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 4 }, AuditReader().GetRevisions(typeof(DoubleListJoinColumnBidirectionalRefEdEntity2), ed2_1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(DoubleListJoinColumnBidirectionalRefEdEntity2), ed2_2_id));
		}


		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1_1_fromRev1 = new DoubleListJoinColumnBidirectionalRefEdEntity1 {Id = ed1_1_id, Data = "ed1_1"};
			var ed1_1_fromRev3 = new DoubleListJoinColumnBidirectionalRefEdEntity1{Id = ed1_1_id, Data = "ed1_1 bis"};
			var ed1_2 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_2_id);
			var ed2_1 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_1_id);
			var ed2_2_fromRev1 = new DoubleListJoinColumnBidirectionalRefEdEntity2 {Id = ed2_2_id, Data = "ed2_2"};
			var ed2_2_fromRev3 = new DoubleListJoinColumnBidirectionalRefEdEntity2{Id = ed2_2_id, Data = "ed2_2 bis"};

			var rev1 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id, 3);
			var rev4 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id, 4);

			CollectionAssert.AreEquivalent(new[] { ed1_1_fromRev1 }, rev1.References1);
			CollectionAssert.AreEquivalent(new[] { ed1_1_fromRev1, ed1_2 }, rev2.References1);
			CollectionAssert.AreEquivalent(new[] { ed1_1_fromRev3, ed1_2 }, rev3.References1);
			CollectionAssert.IsEmpty(rev4.References1);

			CollectionAssert.AreEquivalent(new[] { ed2_1 }, rev1.References2);
			CollectionAssert.AreEquivalent(new[] { ed2_1, ed2_2_fromRev1 }, rev2.References2);
			CollectionAssert.AreEquivalent(new[] { ed2_1, ed2_2_fromRev3 }, rev3.References2);
			CollectionAssert.AreEquivalent(new[] { ed2_2_fromRev3 }, rev4.References2);
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed1_1_fromRev3 = new DoubleListJoinColumnBidirectionalRefEdEntity1 { Id = ed1_1_id, Data = "ed1_1 bis" };
			var ed1_2 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_2_id);
			var ed2_1 = Session.Get<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_1_id);
			var ed2_2_fromRev1 = new DoubleListJoinColumnBidirectionalRefEdEntity2 { Id = ed2_2_id, Data = "ed2_2" };
		
			var rev1 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id, 3);
			var rev4 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id, 4);

			CollectionAssert.AreEquivalent(new[] { ed1_2 }, rev1.References1);
			CollectionAssert.IsEmpty(rev2.References1);
			CollectionAssert.IsEmpty(rev3.References1);
			CollectionAssert.AreEquivalent(new[] { ed1_1_fromRev3, ed1_2 }, rev4.References1);

			CollectionAssert.AreEquivalent(new[] { ed2_2_fromRev1 }, rev1.References2);
			CollectionAssert.IsEmpty(rev2.References2);
			CollectionAssert.IsEmpty(rev3.References2);
			CollectionAssert.AreEquivalent(new[] { ed2_1 }, rev4.References2);
		}

		[Test]
		public void VerifyHistoryOfEd1_1()
		{
			var ing1 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id);
			var ing2 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id);


			var rev1 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_1_id, 1);
			var rev2 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_1_id, 2);
			var rev3 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_1_id, 3);
			var rev4 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_1_id, 4);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing2, rev4.Owner);

			Assert.AreEqual("ed1_1", rev1.Data);
			Assert.AreEqual("ed1_1", rev2.Data);
			Assert.AreEqual("ed1_1 bis", rev3.Data);
			Assert.AreEqual("ed1_1 bis", rev4.Data);
		}

		[Test]
		public void VerifyHistoryOfEd1_2()
		{
			var ing1 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id);
			var ing2 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id);


			var rev1 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_2_id, 1);
			var rev2 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_2_id, 2);
			var rev3 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_2_id, 3);
			var rev4 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity1>(ed1_2_id, 4);

			Assert.AreEqual(ing2, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing2, rev4.Owner);

			Assert.AreEqual("ed1_2", rev1.Data);
			Assert.AreEqual("ed1_2", rev2.Data);
			Assert.AreEqual("ed1_2", rev3.Data);
			Assert.AreEqual("ed1_2", rev4.Data);
		}

		[Test]
		public void VerifyHistoryOfEd2_1()
		{
			var ing1 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id);
			var ing2 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id);


			var rev1 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_1_id, 1);
			var rev2 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_1_id, 2);
			var rev3 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_1_id, 3);
			var rev4 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_1_id, 4);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing2, rev4.Owner);

			Assert.AreEqual("ed2_1", rev1.Data);
			Assert.AreEqual("ed2_1", rev2.Data);
			Assert.AreEqual("ed2_1", rev3.Data);
			Assert.AreEqual("ed2_1", rev4.Data);
		}

		[Test]
		public void VerifyHistoryOfEd2_2()
		{
			var ing1 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing1_id);
			var ing2 = Session.Get<DoubleListJoinColumnBidirectionalRefIngEntity>(ing2_id);


			var rev1 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_2_id, 1);
			var rev2 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_2_id, 2);
			var rev3 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_2_id, 3);
			var rev4 = AuditReader().Find<DoubleListJoinColumnBidirectionalRefEdEntity2>(ed2_2_id, 4);

			Assert.AreEqual(ing2, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing1, rev4.Owner);

			Assert.AreEqual("ed2_2", rev1.Data);
			Assert.AreEqual("ed2_2", rev2.Data);
			Assert.AreEqual("ed2_2 bis", rev3.Data);
			Assert.AreEqual("ed2_2 bis", rev4.Data);
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
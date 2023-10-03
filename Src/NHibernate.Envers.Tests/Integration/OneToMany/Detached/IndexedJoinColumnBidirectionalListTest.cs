using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class IndexedJoinColumnBidirectionalListTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ed3_id;

		private int ing1_id;
		private int ing2_id;


		public IndexedJoinColumnBidirectionalListTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			// Revision 1 (ing1: ed1, ed2, ed3)
			using (var tx = Session.BeginTransaction())
			{
				var ed1 = new IndexedListJoinColumnBidirectionalRefEdEntity { Data = "ed1" };
				var ed2 = new IndexedListJoinColumnBidirectionalRefEdEntity { Data = "ed2" };
				var ed3 = new IndexedListJoinColumnBidirectionalRefEdEntity { Data = "ed3" };

				var ing1 = new IndexedListJoinColumnBidirectionalRefIngEntity { Data = "coll1", References = new List<IndexedListJoinColumnBidirectionalRefEdEntity> { ed1, ed2, ed3 } };
				var ing2 = new IndexedListJoinColumnBidirectionalRefIngEntity { Data = "coll1" };

				ed1_id = (int) Session.Save(ed1);
				ed2_id = (int)Session.Save(ed2);
				ed3_id = (int)Session.Save(ed3);
				ing1_id = (int)Session.Save(ing1);
				ing2_id = (int)Session.Save(ing2);
				tx.Commit();
				Session.Clear();
			}
			// Revision 2 (ing1: ed1, ed3, ing2: ed2)
			using (var tx = Session.BeginTransaction())
			{
				var ing1 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id);
				var ing2 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id);
				var ed2 = Session.Get<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id);
				ing1.References.Remove(ed2);
				ing2.References.Add(ed2);
				tx.Commit();
				Session.Clear();
			}
			// Revision 3 (ing1: ed3, ed1, ing2: ed2)
			using (var tx = Session.BeginTransaction())
			{
				var ing1 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id);
				var ed3 = Session.Get<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id);
				ing1.References.Remove(ed3);
				ing1.References.Insert(0, ed3);
				tx.Commit();
				Session.Clear();
			}
			// Revision 4 (ing1: ed2, ed3, ed1)
			using (var tx = Session.BeginTransaction())
			{
				var ing1 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id);
				var ing2 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id);
				var ed2 = Session.Get<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id);
				ing2.References.Remove(ed2);
				ing1.References.Insert(0, ed2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(IndexedListJoinColumnBidirectionalRefIngEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(IndexedListJoinColumnBidirectionalRefIngEntity), ing2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, AuditReader().GetRevisions(typeof(IndexedListJoinColumnBidirectionalRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(IndexedListJoinColumnBidirectionalRefEdEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(IndexedListJoinColumnBidirectionalRefEdEntity), ed3_id));
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1 = Session.Get<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id);
			var ed2 = Session.Get<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id);
			var ed3 = Session.Get<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id);

			var rev1 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id, 3);
			var rev4 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id, 4);

			CollectionAssert.AreEqual(new[] { ed1, ed2, ed3 }, rev1.References);
			CollectionAssert.AreEqual(new[] { ed1, ed3 }, rev2.References);
			CollectionAssert.AreEqual(new[] { ed3, ed1 }, rev3.References);
			CollectionAssert.AreEqual(new[] { ed2, ed3, ed1 }, rev4.References);
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed2 = Session.Get<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id);

			var rev1 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id, 3);
			var rev4 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id, 4);

			CollectionAssert.IsEmpty(rev1.References);
			CollectionAssert.AreEqual(new[] { ed2 }, rev2.References);
			CollectionAssert.AreEqual(new[] { ed2 }, rev3.References);
			CollectionAssert.IsEmpty(rev4.References);
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id);

			var rev1 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id, 2);
			var rev3 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id, 3);
			var rev4 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id, 4);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing1, rev4.Owner);

			Assert.AreEqual(0, rev1.Position);
			Assert.AreEqual(0, rev2.Position);
			Assert.AreEqual(1, rev3.Position);
			Assert.AreEqual(2, rev4.Position);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id);
			var ing2 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id, 2);
			var rev3 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id, 3);
			var rev4 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id, 4);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing2, rev2.Owner);
			Assert.AreEqual(ing2, rev3.Owner);
			Assert.AreEqual(ing1, rev4.Owner);

			Assert.AreEqual(1, rev1.Position);
			Assert.AreEqual(0, rev2.Position);
			Assert.AreEqual(0, rev3.Position);
			Assert.AreEqual(0, rev4.Position);
		}

		[Test]
		public void VerifyHistoryOfEd3()
		{
			var ing1 = Session.Get<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id);

			var rev1 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id, 1);
			var rev2 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id, 2);
			var rev3 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id, 3);
			var rev4 = AuditReader().Find<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id, 4);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing1, rev4.Owner);

			Assert.AreEqual(2, rev1.Position);
			Assert.AreEqual(1, rev2.Position);
			Assert.AreEqual(0, rev3.Position);
			Assert.AreEqual(1, rev4.Position);
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
using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class JoinColumnBidirectionalListWithInheritanceTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ing1_id;
		private int ing2_id;

		public JoinColumnBidirectionalListWithInheritanceTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1 = new ListJoinColumnBidirectionalInheritanceRefEdChildEntity { ParentData = "ed1", ChildData = "ed1 child" };
			var ed2 = new ListJoinColumnBidirectionalInheritanceRefEdChildEntity { ParentData = "ed2", ChildData = "ed2 child" };
			var ing1 = new ListJoinColumnBidirectionalInheritanceRefIngEntity { Data = "coll1", References = new List<ListJoinColumnBidirectionalInheritanceRefEdParentEntity> { ed1 } };
			var ing2 = new ListJoinColumnBidirectionalInheritanceRefIngEntity { Data = "coll1", References = new List<ListJoinColumnBidirectionalInheritanceRefEdParentEntity> { ed2 } };

			// Revision 1 (ing1: ed1, ing2: ed2)
			using (var tx = Session.BeginTransaction())
			{
				ed1_id = (int)Session.Save(ed1);
				ed2_id = (int)Session.Save(ed2);
				ing1_id = (int)Session.Save(ing1);
				ing2_id = (int)Session.Save(ing2);
				tx.Commit();
			}

			// Revision 2 (ing1: ed1, ed2)
			using (var tx = Session.BeginTransaction())
			{
				ing2.References.Remove(ed2);
				ing1.References.Add(ed2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ListJoinColumnBidirectionalInheritanceRefIngEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ListJoinColumnBidirectionalInheritanceRefIngEntity), ing2_id));

			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(ListJoinColumnBidirectionalInheritanceRefEdParentEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ListJoinColumnBidirectionalInheritanceRefEdParentEntity), ed2_id));
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1 = Session.Get<ListJoinColumnBidirectionalInheritanceRefEdParentEntity>(ed1_id);
			var ed2 = Session.Get<ListJoinColumnBidirectionalInheritanceRefEdParentEntity>(ed2_id);

			var rev1 = AuditReader().Find<ListJoinColumnBidirectionalInheritanceRefIngEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<ListJoinColumnBidirectionalInheritanceRefIngEntity>(ing1_id, 2);

			CollectionAssert.AreEqual(new[] { ed1 }, rev1.References);
			CollectionAssert.AreEqual(new[] { ed1, ed2 }, rev2.References);
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed2 = Session.Get<ListJoinColumnBidirectionalInheritanceRefEdParentEntity>(ed2_id);

			var rev1 = AuditReader().Find<ListJoinColumnBidirectionalInheritanceRefIngEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<ListJoinColumnBidirectionalInheritanceRefIngEntity>(ing2_id, 2);

			CollectionAssert.AreEqual(new[] { ed2 }, rev1.References);
			CollectionAssert.IsEmpty(rev2.References);
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<ListJoinColumnBidirectionalInheritanceRefIngEntity>(ing1_id);

			var rev1 = AuditReader().Find<ListJoinColumnBidirectionalInheritanceRefEdParentEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<ListJoinColumnBidirectionalInheritanceRefEdParentEntity>(ed1_id, 2);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<ListJoinColumnBidirectionalInheritanceRefIngEntity>(ing1_id);
			var ing2 = Session.Get<ListJoinColumnBidirectionalInheritanceRefIngEntity>(ing2_id);

			var rev1 = AuditReader().Find<ListJoinColumnBidirectionalInheritanceRefEdParentEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<ListJoinColumnBidirectionalInheritanceRefEdParentEntity>(ed2_id, 2);

			Assert.AreEqual(ing2, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
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
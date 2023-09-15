using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany
{
	public partial class BasicWhereJoinTableTest : TestBase
	{
		private int ite1_1_id;
		private int ite1_2_id;
		private int ite2_1_id;
		private int ite2_2_id;
		private int wjte1_id;
		private int wjte2_id;

		public BasicWhereJoinTableTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ite1_1_id = 10;
			ite1_2_id = 11;
			ite2_1_id = 20;
			ite2_2_id = 21;
			var ite1_1 = new IntNoAutoIdTestEntity {Number = 1, Id = ite1_1_id};
			var ite1_2 = new IntNoAutoIdTestEntity { Number = 1, Id = ite1_2_id };
			var ite2_1 = new IntNoAutoIdTestEntity { Number = 2, Id = ite2_1_id };
			var ite2_2 = new IntNoAutoIdTestEntity { Number = 2, Id = ite2_2_id };
			var wjte1 = new WhereJoinTableEntity {Data = "wjte1"};
			var wjte2 = new WhereJoinTableEntity {Data = "wjte2"};

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ite1_1);
				Session.Save(ite1_2);
				Session.Save(ite2_1);
				Session.Save(ite2_2);
				wjte1_id = (int)Session.Save(wjte1);
				wjte2_id = (int) Session.Save(wjte2);
				tx.Commit();
			}
			Session.Clear();
			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				wjte1 = Session.Get<WhereJoinTableEntity>(wjte1_id);
				wjte1.References1.Add(ite1_1);
				wjte1.References2.Add(ite2_1);
				tx.Commit();
			}
			Session.Clear();
			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				wjte2 = Session.Get<WhereJoinTableEntity>(wjte2_id);
				wjte2.References1.Add(ite1_1);
				wjte2.References1.Add(ite1_2);
				tx.Commit();
			}
			Session.Clear();
			//rev 4
			using (var tx = Session.BeginTransaction())
			{
				wjte1 = Session.Get<WhereJoinTableEntity>(wjte1_id);
				wjte2 = Session.Get<WhereJoinTableEntity>(wjte2_id);
				wjte1.References1.Remove(ite1_1);
				wjte2.References2.Add(ite2_2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(WhereJoinTableEntity), wjte1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, AuditReader().GetRevisions(typeof(WhereJoinTableEntity), wjte2_id));

			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(IntNoAutoIdTestEntity), ite1_1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(IntNoAutoIdTestEntity), ite1_2_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(IntNoAutoIdTestEntity), ite2_1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(IntNoAutoIdTestEntity), ite2_2_id));
		}

		[Test]
		public void VerifyHistoryOfWjte1()
		{
			var ite1_1 = Session.Get<IntNoAutoIdTestEntity>(ite1_1_id);
			var ite2_1 = Session.Get<IntNoAutoIdTestEntity>(ite2_1_id);

			var rev1 = AuditReader().Find<WhereJoinTableEntity>(wjte1_id, 1);
			var rev2 = AuditReader().Find<WhereJoinTableEntity>(wjte1_id, 2);
			var rev3 = AuditReader().Find<WhereJoinTableEntity>(wjte1_id, 3);
			var rev4 = AuditReader().Find<WhereJoinTableEntity>(wjte1_id, 4);

			// Checking 1st list
			CollectionAssert.IsEmpty(rev1.References1);
			CollectionAssert.AreEquivalent(new[] { ite1_1 }, rev2.References1);
			CollectionAssert.AreEquivalent(new[] { ite1_1 }, rev3.References1);
			CollectionAssert.IsEmpty(rev4.References1);

			// Checking 2nd list
			CollectionAssert.IsEmpty(rev1.References2);
			CollectionAssert.AreEquivalent(new[] { ite2_1 }, rev2.References2);
			CollectionAssert.AreEquivalent(new[] { ite2_1 }, rev3.References2);
			CollectionAssert.AreEquivalent(new[] { ite2_1 }, rev4.References2);
		}

		[Test]
		public void VerifyHistoryOfWjte2()
		{
			var ite1_1 = Session.Get<IntNoAutoIdTestEntity>(ite1_1_id);
			var ite1_2 = Session.Get<IntNoAutoIdTestEntity>(ite1_2_id);
			var ite2_2 = Session.Get<IntNoAutoIdTestEntity>(ite2_2_id);

			var rev1 = AuditReader().Find<WhereJoinTableEntity>(wjte2_id, 1);
			var rev2 = AuditReader().Find<WhereJoinTableEntity>(wjte2_id, 2);
			var rev3 = AuditReader().Find<WhereJoinTableEntity>(wjte2_id, 3);
			var rev4 = AuditReader().Find<WhereJoinTableEntity>(wjte2_id, 4);

			// Checking 1st list
			CollectionAssert.IsEmpty(rev1.References1);
			CollectionAssert.IsEmpty(rev2.References1);
			CollectionAssert.AreEquivalent(new[] { ite1_1, ite1_2 }, rev3.References1);
			CollectionAssert.AreEquivalent(new[] { ite1_1, ite1_2 }, rev4.References1);

			// Checking 2nd list
			CollectionAssert.IsEmpty(rev1.References2);
			CollectionAssert.IsEmpty(rev2.References2);
			CollectionAssert.IsEmpty(rev3.References2);
			CollectionAssert.AreEquivalent(new[] { ite2_2 }, rev4.References2);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.ManyToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}
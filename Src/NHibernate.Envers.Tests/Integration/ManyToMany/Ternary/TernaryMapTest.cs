using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.Ternary
{
	public partial class TernaryMapTest : TestBase
	{
		private int str1_id;
		private int str2_id;
		private int int1_id;
		private int int2_id;
		private int map1_id;
		private int map2_id;

		public TernaryMapTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var str1 = new StrTestPrivSeqEntity { Str = "a" };
			var str2 = new StrTestPrivSeqEntity { Str = "b" };
			var int1 = new IntTestPrivSeqEntity { Number = 1 };
			var int2 = new IntTestPrivSeqEntity { Number = 2 };
			var map1 = new TernaryMapEntity();
			var map2 = new TernaryMapEntity();

			// Revision 1 (map1: initialy one mapping int1 -> str1, map2: empty)
			using (var tx = Session.BeginTransaction())
			{
				str1_id = (int) Session.Save(str1);
				str2_id = (int) Session.Save(str2);
				int1_id = (int) Session.Save(int1);
				int2_id = (int) Session.Save(int2);
				map1.Map[int1] = str1;
				map1_id = (int) Session.Save(map1);
				map2_id = (int) Session.Save(map2);
				tx.Commit();
			}
			// Revision 2 (map1: replacing the mapping, map2: adding two mappings)
			using (var tx = Session.BeginTransaction())
			{
				map1.Map[int1] = str2;
				map2.Map[int1] = str1;
				map2.Map[int2] = str1;
				tx.Commit();
			}
			// Revision 3 (map1: removing a non-existing mapping, adding an existing mapping - no changes, map2: removing a mapping)
			using (var tx = Session.BeginTransaction())
			{
				map1.Map.Remove(int2);
				map1.Map[int1] = str2;
				map2.Map.Remove(int1);
				tx.Commit();
			}
			// Revision 4 (map1: adding a mapping, map2: adding a mapping)
			using (var tx = Session.BeginTransaction())
			{
				map1.Map[int2] = str2;
				map2.Map[int1] = str2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(TernaryMapEntity), map1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(TernaryMapEntity), map2_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestPrivSeqEntity), str1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestPrivSeqEntity), str2_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(IntTestPrivSeqEntity), int1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(IntTestPrivSeqEntity), int2_id));
		}

		[Test]
		public void VerifyHistoryOfMap1()
		{
			var str1 = Session.Get<StrTestPrivSeqEntity>(str1_id);
			var str2 = Session.Get<StrTestPrivSeqEntity>(str2_id);
			var int1 = Session.Get<IntTestPrivSeqEntity>(int1_id);
			var int2 = Session.Get<IntTestPrivSeqEntity>(int2_id);


			var rev1 = AuditReader().Find<TernaryMapEntity>(map1_id, 1);
			var rev2 = AuditReader().Find<TernaryMapEntity>(map1_id, 2);
			var rev3 = AuditReader().Find<TernaryMapEntity>(map1_id, 3);
			var rev4 = AuditReader().Find<TernaryMapEntity>(map1_id, 4);

			CollectionAssert.AreEquivalent(new Dictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity> { { int1, str1 } }, rev1.Map);
			CollectionAssert.AreEquivalent(new Dictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity> { { int1, str2 } }, rev2.Map);
			CollectionAssert.AreEquivalent(new Dictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity> { { int1, str2 } }, rev3.Map);
			CollectionAssert.AreEquivalent(new Dictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity> { { int1, str2 }, { int2, str2 } }, rev4.Map);
		}

		[Test]
		public void VerifyHistoryOfMap2()
		{
			var str1 = Session.Get<StrTestPrivSeqEntity>(str1_id);
			var str2 = Session.Get<StrTestPrivSeqEntity>(str2_id);
			var int1 = Session.Get<IntTestPrivSeqEntity>(int1_id);
			var int2 = Session.Get<IntTestPrivSeqEntity>(int2_id);


			var rev1 = AuditReader().Find<TernaryMapEntity>(map2_id, 1);
			var rev2 = AuditReader().Find<TernaryMapEntity>(map2_id, 2);
			var rev3 = AuditReader().Find<TernaryMapEntity>(map2_id, 3);
			var rev4 = AuditReader().Find<TernaryMapEntity>(map2_id, 4);

			CollectionAssert.IsEmpty(rev1.Map);
			CollectionAssert.AreEquivalent(new Dictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity> { { int1, str1 }, { int2, str1 } }, rev2.Map);
			CollectionAssert.AreEquivalent(new Dictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity> { { int2, str1 } }, rev3.Map);
			CollectionAssert.AreEquivalent(new Dictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity> { { int1, str2 }, { int2, str1 } }, rev4.Map);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"Integration.ManyToMany.Ternary.Mapping.hbm.xml", "Entities.Mapping.hbm.xml"};
			}
		}
	}
}
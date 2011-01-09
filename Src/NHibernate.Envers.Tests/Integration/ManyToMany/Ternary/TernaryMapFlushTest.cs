using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.Ternary
{
	[TestFixture]
	public class TernaryMapFlushTest : TestBase
	{
		private int str1_id;
		private int str2_id;
		private int int1_id;
		private int int2_id;
		private int map1_id;
		
		protected override void Initialize()
		{
			var str1 = new StrTestEntity { Str = "a" };
			var str2 = new StrTestEntity { Str = "b" };
			var int1 = new IntTestEntity { Number = 1 };
			var int2 = new IntTestEntity { Number = 2 };
			var map1 = new TernaryMapEntity();

			// Revision 1 (int1 -> str1)
			using (var tx = Session.BeginTransaction())
			{
				str1_id = (int)Session.Save(str1);
				str2_id = (int)Session.Save(str2);
				int1_id = (int)Session.Save(int1);
				int2_id = (int)Session.Save(int2);
				map1.Map[int1] = str1;
				map1_id = (int)Session.Save(map1);
				tx.Commit();
			}
			// Revision 2 (removing int1->str1, flushing, adding int1->str1 again and a new int2->str2 mapping to force a change)
			using (var tx = Session.BeginTransaction())
			{
				map1.Map = new Dictionary<IntTestEntity, StrTestEntity>();
				Session.Flush();
				map1.Map[int1] = str1;
				map1.Map[int2] = str2;
				tx.Commit();
			}
			// Revision 3 (removing int1->str1, flushing, overwriting int2->str1)
			using (var tx = Session.BeginTransaction())
			{
				map1.Map.Remove(int1);
				Session.Flush();
				map1.Map[int2] = str1;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader.GetRevisions(typeof(TernaryMapEntity), map1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader.GetRevisions(typeof(StrTestEntity), str1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader.GetRevisions(typeof(StrTestEntity), str2_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader.GetRevisions(typeof(IntTestEntity), int1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader.GetRevisions(typeof(IntTestEntity), int2_id));
		}

		[Test]
		public void VerifyHistoryOfMap1()
		{
			var str1 = Session.Get<StrTestEntity>(str1_id);
			var str2 = Session.Get<StrTestEntity>(str2_id);
			var int1 = Session.Get<IntTestEntity>(int1_id);
			var int2 = Session.Get<IntTestEntity>(int2_id);


			var rev1 = AuditReader.Find<TernaryMapEntity>(map1_id, 1);
			var rev2 = AuditReader.Find<TernaryMapEntity>(map1_id, 2);
			var rev3 = AuditReader.Find<TernaryMapEntity>(map1_id, 3);

			CollectionAssert.AreEquivalent(new Dictionary<IntTestEntity, StrTestEntity> { { int1, str1 } }, rev1.Map);
			CollectionAssert.AreEquivalent(new Dictionary<IntTestEntity, StrTestEntity> { { int1, str1 }, {int2, str2} }, rev2.Map);
			CollectionAssert.AreEquivalent(new Dictionary<IntTestEntity, StrTestEntity> { { int2, str1 } }, rev3.Map);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.ManyToMany.Ternary.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}
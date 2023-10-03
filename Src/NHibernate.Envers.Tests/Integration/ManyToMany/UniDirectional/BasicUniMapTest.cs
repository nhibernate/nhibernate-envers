using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.UniDirectional
{
	public partial class BasicUniMapTest : TestBase
	{
		private int str1_id;
		private int str2_id;
		private const int coll1_id = 47;

		public BasicUniMapTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var str1 = new StrTestEntity { Str = "str1" };
			var str2 = new StrTestEntity { Str = "str2" };
			var coll1 = new MapUniEntity {Id = coll1_id, Data = "coll1"};

			using (var tx = Session.BeginTransaction())
			{
				str1_id = (int)Session.Save(str1);
				str2_id = (int)Session.Save(str2);
				coll1.References = new Dictionary<string, StrTestEntity> {{"1", str1}};
				Session.Save(coll1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				coll1.References.Add("2", str2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				coll1.References["2"] = str1;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				coll1.References.Remove("1");
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(MapUniEntity), coll1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), str1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), str2_id));
		}

		[Test]
		public void VerifyHistoryColl1()
		{
			var str1 = Session.Get<StrTestEntity>(str1_id);
			var str2 = Session.Get<StrTestEntity>(str2_id);

			var rev1 = AuditReader().Find<MapUniEntity>(coll1_id, 1);
			var rev2 = AuditReader().Find<MapUniEntity>(coll1_id, 2);
			var rev3 = AuditReader().Find<MapUniEntity>(coll1_id, 3);
			var rev4 = AuditReader().Find<MapUniEntity>(coll1_id, 4);

			CollectionAssert.AreEquivalent(new Dictionary<string, StrTestEntity> {{"1", str1}}, rev1.References);
			CollectionAssert.AreEquivalent(new Dictionary<string, StrTestEntity> {{"1", str1}, {"2", str2}}, rev2.References);
			CollectionAssert.AreEquivalent(new Dictionary<string, StrTestEntity> {{"1", str1}, {"2", str1}}, rev3.References);
			CollectionAssert.AreEquivalent(new Dictionary<string, StrTestEntity> {{"2", str1}}, rev4.References);

			Assert.AreEqual("coll1", rev1.Data);
			Assert.AreEqual("coll1", rev2.Data);
			Assert.AreEqual("coll1", rev3.Data);
			Assert.AreEqual("coll1", rev4.Data);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.ManyToMany.UniDirectional.Mapping.hbm.xml" };
			}
		}
	}
}
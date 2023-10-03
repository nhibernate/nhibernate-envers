using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany
{
	public partial class BasicMapTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;

		private int ing1_id;
		private int ing2_id;

		public BasicMapTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed1_id = 123;
			ed2_id = 444;
			ing1_id = 3;
			ing2_id = 894;

			var ed1 = new MapOwnedEntity { Id = ed1_id, Data = "data_ed_1" };
			var ed2 = new MapOwnedEntity { Id = ed2_id, Data = "data_ed_2" };
			var ing1 = new MapOwningEntity { Id = ing1_id, Data = "data_ing_1" };
			var ing2 = new MapOwningEntity { Id = ing2_id, Data = "data_ing_2" };

			//Rev 1(ing1: initialy empty, ing2: one mapping)
			using (var tx = Session.BeginTransaction())
			{
				ing2.References["2"] = ed2;
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ing1);
				Session.Save(ing2);
				tx.Commit();
			}
			//Rev 2(ing1: adding two (rk - changed to one) mappings, ing2: replacing an existing mapping)
			using (var tx = Session.BeginTransaction())
			{
				ing1.References["1"] = ed1;
				//ing1.References["2"] = ed1; rk - ed1 has a set. cannot understand how did worked in Hib
				ing2.References["2"] = ed1;
				tx.Commit();
			}
			//No revision (ing1: adding an existing mapping, ing2: removing a non existing mapping)
			using (var tx = Session.BeginTransaction())
			{
				ing1.References["1"] = ed1;
				ing2.References.Remove("3");
				tx.Commit();
			}
			//Rev 3(ing1: clearing, ing2: replacing with a new map)
			using (var tx = Session.BeginTransaction())
			{
				ing1.References.Clear();
				ing2.References = new Dictionary<string, MapOwnedEntity>();
				//rk - changed a bit to test multiple items in dic
				ing2.References["1"] = ed1;
				ing2.References["2"] = ed2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(MapOwnedEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(MapOwnedEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(MapOwningEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(MapOwningEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<MapOwningEntity>(ing1_id);
			var ing2 = Session.Get<MapOwningEntity>(ing2_id);

			var rev1 = AuditReader().Find<MapOwnedEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<MapOwnedEntity>(ed1_id, 2);
			var rev3 = AuditReader().Find<MapOwnedEntity>(ed1_id, 3);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev3.Referencing);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing2 = Session.Get<MapOwningEntity>(ing2_id);

			var rev1 = AuditReader().Find<MapOwnedEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<MapOwnedEntity>(ed2_id, 2);
			var rev3 = AuditReader().Find<MapOwnedEntity>(ed2_id, 3);

			CollectionAssert.AreEquivalent(new[] { ing2 }, rev1.Referencing);
			CollectionAssert.IsEmpty(rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev3.Referencing);
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ed1 = Session.Get<MapOwnedEntity>(ed1_id);

			var rev1 = AuditReader().Find<MapOwningEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<MapOwningEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<MapOwningEntity>(ing1_id, 3);

			CollectionAssert.IsEmpty(rev1.References);

			Assert.AreEqual(1, rev2.References.Count);
			Assert.AreEqual(new KeyValuePair<string, MapOwnedEntity>("1", ed1), rev2.References.First());
			
			CollectionAssert.IsEmpty(rev3.References);
		}

		[Test]
		public void VerifyHistoryOfIng2()
		{
			var ed1 = Session.Get<MapOwnedEntity>(ed1_id);
			var ed2 = Session.Get<MapOwnedEntity>(ed2_id);

			var rev1 = AuditReader().Find<MapOwningEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<MapOwningEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<MapOwningEntity>(ing2_id, 3);

			Assert.AreEqual(1, rev1.References.Count);
			Assert.AreEqual(new KeyValuePair<string, MapOwnedEntity>("2", ed2), rev1.References.First());

			Assert.AreEqual(1, rev1.References.Count);
			Assert.AreEqual(new KeyValuePair<string, MapOwnedEntity>("2", ed1), rev2.References.First());

			Assert.AreEqual(2, rev3.References.Count);
			CollectionAssert.Contains(rev3.References, new KeyValuePair<string, MapOwnedEntity>("1", ed1));
			CollectionAssert.Contains(rev3.References, new KeyValuePair<string, MapOwnedEntity>("2", ed2));
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.ManyToMany.Mapping.hbm.xml" };
			}
		}
	}
}
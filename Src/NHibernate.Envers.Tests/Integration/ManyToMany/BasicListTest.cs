using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany
{
	//rk - using bag instead of list. Birefs lists aren't supported in NH Core (?)
	//and also... AFAIK - even Hibernate tests actually use bags.
	public partial class BasicListTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ing1_id;
		private int ing2_id;

		public BasicListTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed1_id = 123;
			ed2_id = 444;
			ing1_id = 3;
			ing2_id = 894;

			var ed1 = new ListOwnedEntity {Id = ed1_id, Data = "data_ed_1"};
			var ed2 = new ListOwnedEntity {Id = ed2_id, Data = "data_ed_2"};
			var ing1 = new ListOwningEntity {Id = ing1_id, Data = "data_ing_1"};
			var ing2 = new ListOwningEntity {Id = ing2_id, Data = "data_ing_2"};

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ing1);
				Session.Save(ing2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.References = new List<ListOwnedEntity> {ed1};
				ing2.References = new List<ListOwnedEntity> {ed1, ed2};
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.References.Add(ed2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.References.Remove(ed1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.References = null;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(ListOwnedEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 5 }, AuditReader().GetRevisions(typeof(ListOwnedEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, AuditReader().GetRevisions(typeof(ListOwningEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ListOwningEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<ListOwningEntity>(ing1_id);
			var ing2 = Session.Get<ListOwningEntity>(ing2_id);

			var rev1 = AuditReader().Find<ListOwnedEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<ListOwnedEntity>(ed1_id, 2);
			var rev3 = AuditReader().Find<ListOwnedEntity>(ed1_id, 3);
			var rev4 = AuditReader().Find<ListOwnedEntity>(ed1_id, 4);
			var rev5 = AuditReader().Find<ListOwnedEntity>(ed1_id, 5);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[]{ing1, ing2}, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[]{ing1, ing2}, rev3.Referencing);
			CollectionAssert.AreEquivalent(new[]{ing2}, rev4.Referencing);
			CollectionAssert.AreEquivalent(new[]{ing2}, rev5.Referencing);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<ListOwningEntity>(ing1_id);
			var ing2 = Session.Get<ListOwningEntity>(ing2_id);

			var rev1 = AuditReader().Find<ListOwnedEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<ListOwnedEntity>(ed2_id, 2);
			var rev3 = AuditReader().Find<ListOwnedEntity>(ed2_id, 3);
			var rev4 = AuditReader().Find<ListOwnedEntity>(ed2_id, 4);
			var rev5 = AuditReader().Find<ListOwnedEntity>(ed2_id, 5);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev3.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev4.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev5.Referencing);
		}

		[Test]
		public void VerifyHistoryIng1()
		{
			var ed1 = Session.Get<ListOwnedEntity>(ed1_id);
			var ed2 = Session.Get<ListOwnedEntity>(ed2_id);

			var rev1 = AuditReader().Find<ListOwningEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<ListOwningEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<ListOwningEntity>(ing1_id, 3);
			var rev4 = AuditReader().Find<ListOwningEntity>(ing1_id, 4);
			var rev5 = AuditReader().Find<ListOwningEntity>(ing1_id, 5);

			CollectionAssert.IsEmpty(rev1.References);
			CollectionAssert.AreEquivalent(new[] { ed1 }, rev2.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev3.References);
			CollectionAssert.AreEquivalent(new[] { ed2 }, rev4.References);
			CollectionAssert.IsEmpty(rev5.References);
		}

		[Test]
		public void VerifyHistoryIng2()
		{
			var ed1 = Session.Get<ListOwnedEntity>(ed1_id);
			var ed2 = Session.Get<ListOwnedEntity>(ed2_id);

			var rev1 = AuditReader().Find<ListOwningEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<ListOwningEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<ListOwningEntity>(ing2_id, 3);
			var rev4 = AuditReader().Find<ListOwningEntity>(ing2_id, 4);
			var rev5 = AuditReader().Find<ListOwningEntity>(ing2_id, 5);

			CollectionAssert.IsEmpty(rev1.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev2.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev3.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev4.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev5.References);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"Entities.ManyToMany.Mapping.hbm.xml"};
			}
		}
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany
{
	public partial class BasicSetTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;

		private int ing1_id;
		private int ing2_id;

		public BasicSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed1_id = 1223;
			ed2_id = 4244;
			ing1_id = 32;
			ing2_id = 8924;

			var ed1 = new SetOwnedEntity {Id = ed1_id, Data = "data_ed_1"};
			var ed2 = new SetOwnedEntity { Id = ed2_id, Data = "data_ed_2" };
			var ing1 = new SetOwningEntity { Id = ing1_id, Data = "data_ing_1" };
			var ing2 = new SetOwningEntity { Id = ing2_id, Data = "data_ing_2" };

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ing1);
				Session.Save(ing2);
				tx.Commit();
			}
			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				ing1.References = new HashSet<SetOwnedEntity> {ed1};
				ing2.References = new HashSet<SetOwnedEntity> {ed1, ed2};
				tx.Commit();
			}
			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				ing1.References.Add(ed2);
				tx.Commit();
			}
			//revision 4
			using (var tx = Session.BeginTransaction())
			{
				ing1.References.Remove(ed1);
				tx.Commit();
			}
			//revision 5
			using (var tx = Session.BeginTransaction())
			{
				ing1.References = null;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(SetOwnedEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 5 }, AuditReader().GetRevisions(typeof(SetOwnedEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, AuditReader().GetRevisions(typeof(SetOwningEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(SetOwningEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ing1 = Session.Get<SetOwningEntity>(ing1_id);
			var ing2 = Session.Get<SetOwningEntity>(ing2_id);

			var rev1 = AuditReader().Find<SetOwnedEntity>(ed1_id, 1);
			var rev2 = AuditReader().Find<SetOwnedEntity>(ed1_id, 2);
			var rev3 = AuditReader().Find<SetOwnedEntity>(ed1_id, 3);
			var rev4 = AuditReader().Find<SetOwnedEntity>(ed1_id, 4);
			var rev5 = AuditReader().Find<SetOwnedEntity>(ed1_id, 5);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev3.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev4.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev5.Referencing);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ing1 = Session.Get<SetOwningEntity>(ing1_id);
			var ing2 = Session.Get<SetOwningEntity>(ing2_id);

			var rev1 = AuditReader().Find<SetOwnedEntity>(ed2_id, 1);
			var rev2 = AuditReader().Find<SetOwnedEntity>(ed2_id, 2);
			var rev3 = AuditReader().Find<SetOwnedEntity>(ed2_id, 3);
			var rev4 = AuditReader().Find<SetOwnedEntity>(ed2_id, 4);
			var rev5 = AuditReader().Find<SetOwnedEntity>(ed2_id, 5);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev3.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing1, ing2 }, rev4.Referencing);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev5.Referencing);
		}

		[Test]
		public void VerifyHistoryIng1()
		{
			var ed1 = Session.Get<SetOwnedEntity>(ed1_id);
			var ed2 = Session.Get<SetOwnedEntity>(ed2_id);

			var rev1 = AuditReader().Find<SetOwningEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<SetOwningEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<SetOwningEntity>(ing1_id, 3);
			var rev4 = AuditReader().Find<SetOwningEntity>(ing1_id, 4);
			var rev5 = AuditReader().Find<SetOwningEntity>(ing1_id, 5);

			CollectionAssert.IsEmpty(rev1.References);
			CollectionAssert.AreEquivalent(new[] { ed1 }, rev2.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev3.References);
			CollectionAssert.AreEquivalent(new[] { ed2 }, rev4.References);
			CollectionAssert.IsEmpty(rev5.References);
		}

		[Test]
		public void VerifyHistoryIng2()
		{
			var ed1 = Session.Get<SetOwnedEntity>(ed1_id);
			var ed2 = Session.Get<SetOwnedEntity>(ed2_id);

			var rev1 = AuditReader().Find<SetOwningEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<SetOwningEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<SetOwningEntity>(ing2_id, 3);
			var rev4 = AuditReader().Find<SetOwningEntity>(ing2_id, 4);
			var rev5 = AuditReader().Find<SetOwningEntity>(ing2_id, 5);

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
				return new[] { "Entities.ManyToMany.Mapping.hbm.xml" };
			}
		}
	}
}
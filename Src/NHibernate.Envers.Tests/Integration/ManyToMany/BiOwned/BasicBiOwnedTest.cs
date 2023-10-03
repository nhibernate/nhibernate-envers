using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.ManyToMany.BiOwned;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.BiOwned
{
	public partial class BasicBiOwnedTest : TestBase
	{
		private int o1_1_id;
		private int o1_2_id;
		private int o2_1_id;
		private int o2_2_id;

		public BasicBiOwnedTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				var o1_1 = new ListBiOwning1Entity();
				var o1_2 = new ListBiOwning1Entity();
				var o2_1 = new ListBiOwning2Entity();
				var o2_2 = new ListBiOwning2Entity();

				o1_1_id = (int) Session.Save(o1_1);
				o1_2_id = (int)Session.Save(o1_2);
				o2_1_id = (int)Session.Save(o2_1);
				o2_2_id = (int)Session.Save(o2_2);
				tx.Commit();
			}
			Session.Clear();
			// Revision 2 (1_1 <-> 2_1; 1_2 <-> 2_2)
			using (var tx = Session.BeginTransaction())
			{
				var o1_1 = Session.Get<ListBiOwning1Entity>(o1_1_id);
				var o1_2 = Session.Get<ListBiOwning1Entity>(o1_2_id);
				var o2_1 = Session.Get<ListBiOwning2Entity>(o2_1_id);
				var o2_2 = Session.Get<ListBiOwning2Entity>(o2_2_id);

				o1_1.Referencing.Add(o2_1);
				o1_2.Referencing.Add(o2_2);
				tx.Commit();
			}
			Session.Clear();
			// Revision 3 (1_1 <-> 2_1, 2_2; 1_2 <-> 2_2)
			using (var tx = Session.BeginTransaction())
			{
				var o1_1 = Session.Get<ListBiOwning1Entity>(o1_1_id);
				var o2_2 = Session.Get<ListBiOwning2Entity>(o2_2_id);

				o1_1.Referencing.Add(o2_2);
				tx.Commit();
			}
			Session.Clear();
			// Revision 4 (1_2 <-> 2_1, 2_2)
			using (var tx = Session.BeginTransaction())
			{
				var o1_1 = Session.Get<ListBiOwning1Entity>(o1_1_id);
				var o1_2 = Session.Get<ListBiOwning1Entity>(o1_2_id);
				var o2_1 = Session.Get<ListBiOwning2Entity>(o2_1_id);
				var o2_2 = Session.Get<ListBiOwning2Entity>(o2_2_id);

				o2_2.Referencing.Remove(o1_1);
				o2_1.Referencing.Remove(o1_1);
				o2_1.Referencing.Add(o1_2);
				tx.Commit();
			}
			Session.Clear();
			// Revision 5 (1_1 <-> 2_2, 1_2 <-> 2_2)
			using (var tx = Session.BeginTransaction())
			{
				var o1_1 = Session.Get<ListBiOwning1Entity>(o1_1_id);
				var o1_2 = Session.Get<ListBiOwning1Entity>(o1_2_id);
				var o2_1 = Session.Get<ListBiOwning2Entity>(o2_1_id);
				var o2_2 = Session.Get<ListBiOwning2Entity>(o2_2_id);

				o1_2.Referencing.Remove(o2_1);
				o1_1.Referencing.Add(o2_2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			// Although it would seem that when modifying references both entities should be marked as modified, because
			// ownly the owning side is notified (because of the bi-owning mapping), a revision is created only for
			// the entity where the collection was directly modified.
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 5 }, AuditReader().GetRevisions(typeof(ListBiOwning1Entity), o1_1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 5 }, AuditReader().GetRevisions(typeof(ListBiOwning1Entity), o1_2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 4 }, AuditReader().GetRevisions(typeof(ListBiOwning2Entity), o2_1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 4 }, AuditReader().GetRevisions(typeof(ListBiOwning2Entity), o2_2_id));
		}

		[Test]
		public void VerifyHistoryOfO1_1()
		{
			var o2_1 = Session.Get<ListBiOwning2Entity>(o2_1_id);
			var o2_2 = Session.Get<ListBiOwning2Entity>(o2_2_id);

			var rev1 = AuditReader().Find<ListBiOwning1Entity>(o1_1_id, 1);
			var rev2 = AuditReader().Find<ListBiOwning1Entity>(o1_1_id, 2);
			var rev3 = AuditReader().Find<ListBiOwning1Entity>(o1_1_id, 3);
			var rev4 = AuditReader().Find<ListBiOwning1Entity>(o1_1_id, 4);
			var rev5 = AuditReader().Find<ListBiOwning1Entity>(o1_1_id, 5);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[] { o2_1 }, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { o2_1, o2_2 }, rev3.Referencing);
			CollectionAssert.IsEmpty(rev4.Referencing);
			CollectionAssert.AreEquivalent(new[] { o2_2 }, rev5.Referencing);
		}

		[Test]
		public void VerifyHistoryOfO1_2()
		{
			var o2_1 = Session.Get<ListBiOwning2Entity>(o2_1_id);
			var o2_2 = Session.Get<ListBiOwning2Entity>(o2_2_id);

			var rev1 = AuditReader().Find<ListBiOwning1Entity>(o1_2_id, 1);
			var rev2 = AuditReader().Find<ListBiOwning1Entity>(o1_2_id, 2);
			var rev3 = AuditReader().Find<ListBiOwning1Entity>(o1_2_id, 3);
			var rev4 = AuditReader().Find<ListBiOwning1Entity>(o1_2_id, 4);
			var rev5 = AuditReader().Find<ListBiOwning1Entity>(o1_2_id, 5);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[] { o2_2 }, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { o2_2 }, rev3.Referencing);
			CollectionAssert.AreEquivalent(new[] { o2_1, o2_2 }, rev4.Referencing);
			CollectionAssert.AreEquivalent(new[] { o2_2 }, rev5.Referencing);
		}

		[Test]
		public void VerifyHistoryOfO2_1()
		{
			var o1_1 = Session.Get<ListBiOwning1Entity>(o1_1_id);
			var o1_2 = Session.Get<ListBiOwning1Entity>(o1_2_id);

			var rev1 = AuditReader().Find<ListBiOwning2Entity>(o2_1_id, 1);
			var rev2 = AuditReader().Find<ListBiOwning2Entity>(o2_1_id, 2);
			var rev3 = AuditReader().Find<ListBiOwning2Entity>(o2_1_id, 3);
			var rev4 = AuditReader().Find<ListBiOwning2Entity>(o2_1_id, 4);
			var rev5 = AuditReader().Find<ListBiOwning2Entity>(o2_1_id, 5);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[] { o1_1 }, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { o1_1 }, rev3.Referencing);
			CollectionAssert.AreEquivalent(new[] { o1_2 }, rev4.Referencing);
			CollectionAssert.IsEmpty(rev5.Referencing);
		}

		[Test]
		public void VerifyHistoryOfO2_2()
		{
			var o1_1 = Session.Get<ListBiOwning1Entity>(o1_1_id);
			var o1_2 = Session.Get<ListBiOwning1Entity>(o1_2_id);

			var rev1 = AuditReader().Find<ListBiOwning2Entity>(o2_2_id, 1);
			var rev2 = AuditReader().Find<ListBiOwning2Entity>(o2_2_id, 2);
			var rev3 = AuditReader().Find<ListBiOwning2Entity>(o2_2_id, 3);
			var rev4 = AuditReader().Find<ListBiOwning2Entity>(o2_2_id, 4);
			var rev5 = AuditReader().Find<ListBiOwning2Entity>(o2_2_id, 5);

			CollectionAssert.IsEmpty(rev1.Referencing);
			CollectionAssert.AreEquivalent(new[] { o1_2 }, rev2.Referencing);
			CollectionAssert.AreEquivalent(new[] { o1_1, o1_2 }, rev3.Referencing);
			CollectionAssert.AreEquivalent(new[] { o1_2 }, rev4.Referencing);
			CollectionAssert.AreEquivalent(new[] { o1_1, o1_2 }, rev5.Referencing);
		}


		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.ManyToMany.BiOwned.Mapping.hbm.xml" };
			}
		}
	}
}
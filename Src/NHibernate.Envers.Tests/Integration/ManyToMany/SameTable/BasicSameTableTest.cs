using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.ManyToMany.SameTable;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.SameTable
{
	public class BasicSameTableTest : TestBase
	{
		private int p1_id;
		private int p2_id;
		private int c1_1_id;
		private int c1_2_id;
		private int c2_1_id;
		private int c2_2_id;

		public BasicSameTableTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var intType = Dialect.GetTypeName(SqlTypeFactory.Int32);
			var tinyIntType = Dialect.GetTypeName(SqlTypeFactory.Byte);

			// We need first to modify the columns in the middle (join table) to allow null values. Hbm2ddl doesn't seem
			// to allow this.
			using (var tx = Session.BeginTransaction())
			{
				Session.CreateSQLQuery("DROP TABLE children").ExecuteUpdate();
				Session.CreateSQLQuery("CREATE TABLE children(parent_id " + intType + ", child1_id " + intType + ", child2_id " + intType + ")").ExecuteUpdate();
				Session.CreateSQLQuery("DROP TABLE children_AUD").ExecuteUpdate();
				Session.CreateSQLQuery("CREATE TABLE children_AUD(REV " + intType + " NOT NULL, REVEND " + intType + ", REVTYPE " + tinyIntType + ", " +
						"parent_id " + intType + ", child1_id " + intType + ", child2_id " + intType + ")").ExecuteUpdate();
				tx.Commit();
			}

			// Revision 1
			using (var tx = Session.BeginTransaction())
			{
				var p1 = new ParentEntity();
				var p2 = new ParentEntity();
				var c1_1 = new Child1Entity();
				var c1_2 = new Child1Entity();
				var c2_1 = new Child2Entity();
				var c2_2 = new Child2Entity();

				p1_id = (int)Session.Save(p1);
				p2_id = (int)Session.Save(p2);
				c1_1_id = (int) Session.Save(c1_1);
				c1_2_id = (int) Session.Save(c1_2);
				c2_1_id = (int) Session.Save(c2_1);
				c2_2_id = (int) Session.Save(c2_2);
				tx.Commit();
			}
			Session.Clear();
			// revision 2 - (p1: c1_1, p2: c2_1)
			using (var tx = Session.BeginTransaction())
			{
				var p1 = Session.Get<ParentEntity>(p1_id);
				var p2 = Session.Get<ParentEntity>(p2_id);
				var c1_1 = Session.Get<Child1Entity>(c1_1_id);
				var c2_1 = Session.Get<Child2Entity>(c2_1_id);
				p1.Children1.Add(c1_1);
				p2.Children2.Add(c2_1);
				tx.Commit();
			}
			Session.Clear();
			// revision 3 - (p1: c1_1, c1_2, c2_2, p2: c1_1, c2_1)
			using (var tx = Session.BeginTransaction())
			{
				var p1 = Session.Get<ParentEntity>(p1_id);
				var p2 = Session.Get<ParentEntity>(p2_id);
				var c1_1 = Session.Get<Child1Entity>(c1_1_id);
				var c1_2 = Session.Get<Child1Entity>(c1_2_id);
				var c2_2 = Session.Get<Child2Entity>(c2_2_id);
				p1.Children1.Add(c1_2);
				p1.Children2.Add(c2_2);
				p2.Children1.Add(c1_1);
				tx.Commit();
			}
			Session.Clear();
			// revision 4 - (p1: c1_2, c2_2, p2: c1_1, c2_1, c2_2)
			using (var tx = Session.BeginTransaction())
			{
				var p1 = Session.Get<ParentEntity>(p1_id);
				var p2 = Session.Get<ParentEntity>(p2_id);
				var c1_1 = Session.Get<Child1Entity>(c1_1_id);
				var c2_2 = Session.Get<Child2Entity>(c2_2_id);
				p1.Children1.Remove(c1_1);
				p2.Children2.Add(c2_2);
				tx.Commit();
			}
			Session.Clear();
			// Revision 5 - (p1: c2_2, p2: c1_1, c2_1)
			using (var tx = Session.BeginTransaction())
			{
				var p1 = Session.Get<ParentEntity>(p1_id);
				var p2 = Session.Get<ParentEntity>(p2_id);
				var c1_2 = Session.Get<Child1Entity>(c1_2_id);
				var c2_2 = Session.Get<Child2Entity>(c2_2_id);

				c2_2.Parents.Remove(p2);
				c1_2.Parents.Remove(p1);
				p2.Children2.Add(c2_2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(ParentEntity), p1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(ParentEntity), p2_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(Child1Entity), c1_1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 5 }, AuditReader().GetRevisions(typeof(Child1Entity), c1_2_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(Child2Entity), c2_1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 5 }, AuditReader().GetRevisions(typeof(Child2Entity), c2_2_id));
		}

		[Test]
		public void VerifyHistoryOfParent1()
		{
			var c1_1 = Session.Get<Child1Entity>(c1_1_id);
			var c1_2 = Session.Get<Child1Entity>(c1_2_id);
			var c2_2 = Session.Get<Child2Entity>(c2_2_id);

			var rev1 = AuditReader().Find<ParentEntity>(p1_id, 1);
			var rev2 = AuditReader().Find<ParentEntity>(p1_id, 2);
			var rev3 = AuditReader().Find<ParentEntity>(p1_id, 3);
			var rev4 = AuditReader().Find<ParentEntity>(p1_id, 4);
			var rev5 = AuditReader().Find<ParentEntity>(p1_id, 5);

			CollectionAssert.IsEmpty(rev1.Children1);
			CollectionAssert.AreEquivalent(new[] { c1_1 }, rev2.Children1);
			CollectionAssert.AreEquivalent(new[] { c1_1, c1_2 }, rev3.Children1);
			CollectionAssert.AreEquivalent(new[] { c1_2 }, rev4.Children1);
			CollectionAssert.IsEmpty(rev5.Children1);

			CollectionAssert.IsEmpty(rev1.Children2);
			CollectionAssert.IsEmpty(rev2.Children2);
			CollectionAssert.AreEquivalent(new[] { c2_2 }, rev3.Children2);
			CollectionAssert.AreEquivalent(new[] { c2_2 }, rev4.Children2);
			CollectionAssert.AreEquivalent(new[] { c2_2 }, rev5.Children2);
		}

		[Test]
		public void VerifyHistoryOfParent2()
		{
			var c1_1 = Session.Get<Child1Entity>(c1_1_id);
			var c2_1 = Session.Get<Child2Entity>(c2_1_id);
			var c2_2 = Session.Get<Child2Entity>(c2_2_id);

			var rev1 = AuditReader().Find<ParentEntity>(p2_id, 1);
			var rev2 = AuditReader().Find<ParentEntity>(p2_id, 2);
			var rev3 = AuditReader().Find<ParentEntity>(p2_id, 3);
			var rev4 = AuditReader().Find<ParentEntity>(p2_id, 4);
			var rev5 = AuditReader().Find<ParentEntity>(p2_id, 5);

			CollectionAssert.IsEmpty(rev1.Children1);
			CollectionAssert.IsEmpty(rev2.Children1);
			CollectionAssert.AreEquivalent(new[] { c1_1 }, rev3.Children1);
			CollectionAssert.AreEquivalent(new[] { c1_1 }, rev4.Children1);
			CollectionAssert.AreEquivalent(new[] { c1_1 }, rev5.Children1);

			CollectionAssert.IsEmpty(rev1.Children2);
			CollectionAssert.AreEquivalent(new[] { c2_1 }, rev2.Children2);
			CollectionAssert.AreEquivalent(new[] { c2_1 }, rev3.Children2);
			CollectionAssert.AreEquivalent(new[] { c2_1, c2_2 }, rev4.Children2);
			CollectionAssert.AreEquivalent(new[] { c2_1 }, rev5.Children2);
		}

		[Test]
		public void VerifyHistoryOfChild1_1()
		{
			var p1 = Session.Get<ParentEntity>(p1_id);
			var p2 = Session.Get<ParentEntity>(p2_id);

			var rev1 = AuditReader().Find<Child1Entity>(c1_1_id, 1);
			var rev2 = AuditReader().Find<Child1Entity>(c1_1_id, 2);
			var rev3 = AuditReader().Find<Child1Entity>(c1_1_id, 3);
			var rev4 = AuditReader().Find<Child1Entity>(c1_1_id, 4);
			var rev5 = AuditReader().Find<Child1Entity>(c1_1_id, 5);

			CollectionAssert.IsEmpty(rev1.Parents);
			CollectionAssert.AreEquivalent(new[] { p1 }, rev2.Parents);
			CollectionAssert.AreEquivalent(new[] { p1, p2 }, rev3.Parents);
			CollectionAssert.AreEquivalent(new[] { p2 }, rev4.Parents);
			CollectionAssert.AreEquivalent(new[] { p2 }, rev5.Parents);
		}

		[Test]
		public void VerifyHistoryOfChild1_2()
		{
			var p1 = Session.Get<ParentEntity>(p1_id);

			var rev1 = AuditReader().Find<Child1Entity>(c1_2_id, 1);
			var rev2 = AuditReader().Find<Child1Entity>(c1_2_id, 2);
			var rev3 = AuditReader().Find<Child1Entity>(c1_2_id, 3);
			var rev4 = AuditReader().Find<Child1Entity>(c1_2_id, 4);
			var rev5 = AuditReader().Find<Child1Entity>(c1_2_id, 5);

			CollectionAssert.IsEmpty(rev1.Parents);
			CollectionAssert.IsEmpty(rev2.Parents);
			CollectionAssert.AreEquivalent(new[] { p1 }, rev3.Parents);
			CollectionAssert.AreEquivalent(new[] { p1 }, rev4.Parents);
			CollectionAssert.IsEmpty(rev5.Parents);
		}

		[Test]
		public void VerifyHistoryOfChild2_1()
		{
			var p2 = Session.Get<ParentEntity>(p2_id);

			var rev1 = AuditReader().Find<Child2Entity>(c2_1_id, 1);
			var rev2 = AuditReader().Find<Child2Entity>(c2_1_id, 2);
			var rev3 = AuditReader().Find<Child2Entity>(c2_1_id, 3);
			var rev4 = AuditReader().Find<Child2Entity>(c2_1_id, 4);
			var rev5 = AuditReader().Find<Child2Entity>(c2_1_id, 5);

			CollectionAssert.IsEmpty(rev1.Parents);
			CollectionAssert.AreEquivalent(new[] { p2 }, rev2.Parents);
			CollectionAssert.AreEquivalent(new[] { p2 }, rev3.Parents);
			CollectionAssert.AreEquivalent(new[] { p2 }, rev4.Parents);
			CollectionAssert.AreEquivalent(new[] { p2 }, rev5.Parents);
		}

		[Test]
		public void VerifyHistoryOfChild2_2()
		{
			var p1 = Session.Get<ParentEntity>(p1_id);
			var p2 = Session.Get<ParentEntity>(p2_id);

			var rev1 = AuditReader().Find<Child2Entity>(c2_2_id, 1);
			var rev2 = AuditReader().Find<Child2Entity>(c2_2_id, 2);
			var rev3 = AuditReader().Find<Child2Entity>(c2_2_id, 3);
			var rev4 = AuditReader().Find<Child2Entity>(c2_2_id, 4);
			var rev5 = AuditReader().Find<Child2Entity>(c2_2_id, 5);

			CollectionAssert.IsEmpty(rev1.Parents);
			CollectionAssert.IsEmpty(rev2.Parents);
			CollectionAssert.AreEquivalent(new[] { p1 }, rev3.Parents);
			CollectionAssert.AreEquivalent(new[] { p1, p2 }, rev4.Parents);
			CollectionAssert.AreEquivalent(new[] { p1 }, rev5.Parents);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"Entities.ManyToMany.SameTable.Mapping.hbm.xml"};
			}
		}
	}
}
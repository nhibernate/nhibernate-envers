using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities.ManyToMany.SameTable;
using NHibernate.Envers.Tests.Entities.RevEntity;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Strategy
{
	public class ValidityAuditStrategyRevEndTimestampTest : ValidityTestBase
	{
		private const string revendTimestampColumName = "REVEND_TIMESTAMP";

		private int p1_id;
		private int p2_id;
		private int c1_1_id;
		private int c1_2_id;
		private int c2_1_id;
		private int c2_2_id;

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.AuditStrategyValidityStoreRevendTimestamp, true)
				.SetEnversProperty(ConfigurationKey.AuditStrategyValidityRevendTimestampFieldName, revendTimestampColumName);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.ManyToMany.SameTable.Mapping.hbm.xml",
								"Entities.RevEntity.CustomRevEntity.hbm.xml"};
			}
		}

		protected override void Initialize()
		{
			var p1 = new ParentEntity();
			var p2 = new ParentEntity();
			var c1_1 = new Child1Entity();
			var c1_2 = new Child1Entity();
			var c2_1 = new Child2Entity();
			var c2_2 = new Child2Entity();

			allowNullInMiddleTable();

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				p1_id = (int)Session.Save(p1);
				p2_id = (int)Session.Save(p2);
				c1_1_id = (int)Session.Save(c1_1);
				c1_2_id = (int)Session.Save(c1_2);
				c2_1_id = (int)Session.Save(c2_1);
				c2_2_id = (int)Session.Save(c2_2);
				tx.Commit();
			}

			//rev 2 - (p1: c1_1, p2: c2_1)
			using (var tx = Session.BeginTransaction())
			{
				p1.Children1.Add(c1_1);
				p2.Children2.Add(c2_1);
				tx.Commit();
			}

			// Rev 3 - (p1: c1_1, c1_2, c2_2, p2: c1_1, c2_1)
			using (var tx = Session.BeginTransaction())
			{
				p1.Children1.Add(c1_2);
				p1.Children2.Add(c2_2);
				p2.Children1.Add(c1_1);
				tx.Commit();
			}

			// Revision 4 - (p1: c1_2, c2_2, p2: c1_1, c2_1, c2_2)
			using (var tx = Session.BeginTransaction())
			{
				p1.Children1.Remove(c1_1);
				p2.Children2.Add(c2_2);
				tx.Commit();
			}

			// Rev 5 - (p1: c2_2, p2: c1_1, c2_1)
			Session.Clear();
			using (var tx = Session.BeginTransaction())
			{
				c1_2 = Session.Get<Child1Entity>(c1_2.Id);
				c2_2 = Session.Get<Child2Entity>(c2_2.Id);

				c2_2.Parents.Remove(p2);
				c1_2.Parents.Remove(p1);
				tx.Commit();
			}
		}

		private void allowNullInMiddleTable()
		{
			using (var tx = Session.BeginTransaction())
			{
				Session.CreateSQLQuery("DROP TABLE children").ExecuteUpdate();
				Session.CreateSQLQuery("CREATE TABLE children(parent_id integer, child1_id integer NULL, child2_id integer NULL)").ExecuteUpdate();
				Session.CreateSQLQuery("DROP TABLE children_aud").ExecuteUpdate();
				Session.CreateSQLQuery("CREATE TABLE children_AUD(REV integer NOT NULL, REVEND integer, "
										+ revendTimestampColumName
										+ " datetime, REVTYPE tinyint, "
										+ "parent_id integer, child1_id integer NULL, child2_id integer NULL)").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void FindRevisions()
		{
			var revNumbers = new List<long> { 1, 2, 3, 4, 5 };
			AuditReader().FindRevisions<CustomRevEntity>(revNumbers)
				.Should().Have.Count.EqualTo(5);
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(ParentEntity),p1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(ParentEntity),p2_id));

			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(Child1Entity),c1_1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 5 }, AuditReader().GetRevisions(typeof(Child1Entity),c1_2_id));

			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(Child2Entity),c2_1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 5 }, AuditReader().GetRevisions(typeof(Child2Entity),c2_2_id));
		}

		[Test]
		public void VerifyAllRevEndTimeStamps()
		{
			var p1RevList = getRevisions(typeof(ParentEntity), p1_id);
			var p2RevList = getRevisions(typeof(ParentEntity), p2_id);
			var c1_1_List = getRevisions(typeof(ParentEntity), c1_1_id);
			var c1_2_List = getRevisions(typeof(ParentEntity), c1_2_id);
			var c2_1_List = getRevisions(typeof(ParentEntity), c2_1_id);
			var c2_2_List = getRevisions(typeof(ParentEntity), c2_2_id);

			verifyRevEndTimeStamps(p1RevList);
			verifyRevEndTimeStamps(p2RevList);
			verifyRevEndTimeStamps(c1_1_List);
			verifyRevEndTimeStamps(c1_2_List);
			verifyRevEndTimeStamps(c2_1_List);
			verifyRevEndTimeStamps(c2_2_List);
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
			CollectionAssert.AreEqual(new[] { c1_1 }, rev2.Children1);
			CollectionAssert.AreEqual(new[] { c1_1, c1_2 }, rev3.Children1);
			CollectionAssert.AreEqual(new[] { c1_2 }, rev4.Children1);
			CollectionAssert.IsEmpty(rev5.Children1);

			CollectionAssert.IsEmpty(rev1.Children2);
			CollectionAssert.IsEmpty(rev2.Children2);
			CollectionAssert.AreEqual(new[] { c2_2 }, rev3.Children2);
			CollectionAssert.AreEqual(new[] { c2_2 }, rev4.Children2);
			CollectionAssert.AreEqual(new[] { c2_2 }, rev5.Children2);
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
			CollectionAssert.AreEqual(new[] { c1_1 }, rev3.Children1);
			CollectionAssert.AreEqual(new[] { c1_1 }, rev4.Children1);
			CollectionAssert.AreEqual(new[] { c1_1 }, rev5.Children1);

			CollectionAssert.IsEmpty(rev1.Children2);
			CollectionAssert.AreEqual(new[] { c2_1 }, rev2.Children2);
			CollectionAssert.AreEqual(new[] { c2_1 }, rev3.Children2);
			CollectionAssert.AreEqual(new[] { c2_1, c2_2 }, rev4.Children2);
			CollectionAssert.AreEqual(new[] { c2_1 }, rev5.Children2);
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
			CollectionAssert.AreEqual(new[] { p1 }, rev2.Parents);
			CollectionAssert.AreEqual(new[] { p1, p2 }, rev3.Parents);
			CollectionAssert.AreEqual(new[] { p2 }, rev4.Parents);
			CollectionAssert.AreEqual(new[] { p2 }, rev5.Parents);
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
			CollectionAssert.AreEqual(new[] { p1 }, rev3.Parents);
			CollectionAssert.AreEqual(new[] { p1 }, rev4.Parents);
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
			CollectionAssert.AreEqual(new[] { p2 }, rev2.Parents);
			CollectionAssert.AreEqual(new[] { p2 }, rev3.Parents);
			CollectionAssert.AreEqual(new[] { p2 }, rev4.Parents);
			CollectionAssert.AreEqual(new[] { p2 }, rev5.Parents);
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
			CollectionAssert.AreEqual(new[] { p1 }, rev3.Parents);
			CollectionAssert.AreEqual(new[] { p1, p2 }, rev4.Parents);
			CollectionAssert.AreEqual(new[] { p1 }, rev5.Parents);
		}

		private IEnumerable<IDictionary> getRevisions(System.Type originalEntityClazz, int originalEntityId)
		{
			// Build the query:
			// select auditEntity from
			// org.hibernate.envers.test.entities.manytomany.sametable.ParentEntity_AUD
			// auditEntity where auditEntity.originalId.id = :originalEntityId

			var builder = new StringBuilder("select auditEntity from ");
			builder.Append(originalEntityClazz.FullName).Append("_AUD auditEntity");
			builder.Append(" where auditEntity.originalId.Id = :originalEntityId");

			var qry = Session.CreateQuery(builder.ToString());
			qry.SetParameter("originalEntityId", originalEntityId);

			return qry.List<IDictionary>();
		}

		private static void verifyRevEndTimeStamps(IEnumerable<IDictionary> revisionEntities)
		{
			foreach (var revisionEntity in revisionEntities)
			{

				var revendTimestamp = revisionEntity[revendTimestampColumName];
				var revEnd = (CustomRevEntity)revisionEntity["REVEND"];

				if (revendTimestamp == null)
				{
					revEnd.Should().Be.Null();
				}
				else
				{
					var exactDate = new DateTime(revEnd.CustomTimestamp);
					var revendDate = (DateTime) revendTimestamp;
					revendDate.Should().Be.IncludedIn(exactDate.AddMilliseconds(-100), exactDate.AddMilliseconds(100));
				}
			}
		}
	}
}
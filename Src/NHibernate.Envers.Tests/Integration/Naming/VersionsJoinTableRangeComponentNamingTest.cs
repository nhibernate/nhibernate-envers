using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public partial class VersionsJoinTableRangeComponentNamingTest : TestBase
	{
		private int vjrcte_id;
		private int vjtrte_id;
		private int vjtrtae_id1;

		public VersionsJoinTableRangeComponentNamingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var vjrcte = new VersionsJoinTableRangeComponentTestEntity();
			using (var tx = Session.BeginTransaction())
			{
				vjrcte_id = (int) Session.Save(vjrcte);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				// create a component containing a list of
				// VersionsJoinTableRangeTestEntity-instances
				var vjtrte = new VersionsJoinTableRangeTestEntity {GenericValue = "generic1", Value = "value1"};
				// and add it to the test entity
				vjrcte.Component1.Range.Add(vjtrte);

				// create a second component containing a list of
				// VersionsJoinTableRangeTestAlternateEntity-instances
				var vjtrtae1 = new VersionsJoinTableRangeTestAlternateEntity { GenericValue = "generic2", AlternativeValue = "alternateValue2" };
				vjrcte.Component2.Range.Add(vjtrtae1);

				// create a third component, and add it to the test entity
				var simpleComponent = new Component1 {Str1 = "string1", Str2 = "string2"};
				vjrcte.Component3 = simpleComponent;

				vjtrte_id = (int) Session.Save(vjtrte);
				vjtrtae_id1 = (int) Session.Save(vjtrtae1);
				Session.SaveOrUpdate(vjrcte);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(VersionsJoinTableRangeComponentTestEntity), vjrcte_id));
			CollectionAssert.AreEquivalent(new[] { 2 }, AuditReader().GetRevisions(typeof(VersionsJoinTableRangeTestEntity), vjtrte_id));
			CollectionAssert.AreEquivalent(new[] { 2 }, AuditReader().GetRevisions(typeof(VersionsJoinTableRangeTestAlternateEntity), vjtrtae_id1));
		}

		[Test]
		public void VerifyHistoryUfUniId1()
		{
			var vjtrte = Session.Get<VersionsJoinTableRangeTestEntity>(vjtrte_id);
			var vjtrtae = Session.Get<VersionsJoinTableRangeTestAlternateEntity>(vjtrtae_id1);

			var rev1 = AuditReader().Find<VersionsJoinTableRangeComponentTestEntity>(vjrcte_id, 1);
			var rev2 = AuditReader().Find<VersionsJoinTableRangeComponentTestEntity>(vjrcte_id, 2);

			CollectionAssert.IsEmpty(rev1.Component1.Range);
			CollectionAssert.IsEmpty(rev1.Component2.Range);
			CollectionAssert.AreEquivalent(new[] { vjtrte }, rev2.Component1.Range);
			CollectionAssert.AreEquivalent(new[] { vjtrtae }, rev2.Component2.Range);
		}

		// The Audit join tables we expect
		private const string COMPONENT_1_AUDIT_JOIN_TABLE_NAME = "JOIN_TABLE_COMPONENT_1_AUD";
		private const string COMPONENT_2_AUDIT_JOIN_TABLE_NAME = "JOIN_TABLE_COMPONENT_2_AUD";

		// The Audit join tables that should NOT be there
		private const string UNMODIFIED_COMPONENT_1_AUDIT_JOIN_TABLE_NAME = "VersionsJoinTableRangeComponentTestEntity_VersionsJoinTableRangeTestEntity_AUD";
		private const string UNMODIFIED_COMPONENT_2_AUDIT_JOIN_TABLE_NAME = "VersionsJoinTableRangeComponentTestEntity_VersionsJoinTableRangeTestAlternateEntity_AUD";

		[Test]
		public void ExpectedTableNamesForComponent1()
		{
			var auditClass = Cfg.GetClassMapping(COMPONENT_1_AUDIT_JOIN_TABLE_NAME);
			Assert.IsNotNull(auditClass);
			Assert.AreEqual(COMPONENT_1_AUDIT_JOIN_TABLE_NAME, auditClass.Table.Name);
		}

		[Test]
		public void ExpectedTableNamesForComponent2()
		{
			var auditClass = Cfg.GetClassMapping(COMPONENT_2_AUDIT_JOIN_TABLE_NAME);
			Assert.IsNotNull(auditClass);
			Assert.AreEqual(COMPONENT_2_AUDIT_JOIN_TABLE_NAME, auditClass.Table.Name);
		}

		[Test]
		public void WrongTableNamesForComponent1()
		{
			var auditClass = Cfg.GetClassMapping(UNMODIFIED_COMPONENT_1_AUDIT_JOIN_TABLE_NAME);
			Assert.IsNull(auditClass);
		}

		[Test]
		public void WrongTableNamesForComponent2()
		{
			var auditClass = Cfg.GetClassMapping(UNMODIFIED_COMPONENT_2_AUDIT_JOIN_TABLE_NAME);
			Assert.IsNull(auditClass);
		}

		[Test]
		public void VerifyColumnNamesForComponent1()
		{
			var auditClass = Cfg.GetClassMapping(COMPONENT_1_AUDIT_JOIN_TABLE_NAME);

			var id1Found = false;
			var id2Found = false;
			foreach (var column in auditClass.Table.ColumnIterator)
			{
				if (column.Name.Equals("VJTRCTE1_ID"))
					id1Found = true;
				if (column.Name.Equals("VJTRTE_ID"))
					id2Found = true;
			}
			Assert.IsTrue(id1Found && id2Found);
		}

		[Test]
		public void VerifyColumnNamesForComponent2()
		{
			var auditClass = Cfg.GetClassMapping(COMPONENT_2_AUDIT_JOIN_TABLE_NAME);

			var id1Found = false;
			var id2Found = false;
			foreach (var column in auditClass.Table.ColumnIterator)
			{
				if (column.Name.Equals("VJTRCTE2_ID"))
					id1Found = true;
				if (column.Name.Equals("VJTRTAE_ID"))
					id2Found = true;
			}
			Assert.IsTrue(id1Found && id2Found);
		}

		[Test]
		public void VerifyOverrideNotAudited()
		{
			var auditClass = Cfg.GetClassMapping(typeof (VersionsJoinTableRangeComponentTestEntity).FullName + "_AUD");
			var auditColumn1Found = false;
			var auditColumn2Found = false;
			foreach (var column in auditClass.Table.ColumnIterator)
			{
				if (column.Name.Equals("STR1"))
					auditColumn1Found = true;
				if (column.Name.Equals("STR2"))
					auditColumn2Found = true;
			}
			Assert.IsTrue(auditColumn1Found && !auditColumn2Found);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Naming.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}
using System;
using System.Collections.Generic;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration
{
	public class GlobalConfiguration 
	{
		public GlobalConfiguration(IDictionary<string,string> properties) 
		{
			var generateRevisionsForCollectionsStr = Toolz.GetProperty(properties,
					"envers.revision_on_collection_change",
					"true");
			GenerateRevisionsForCollections = Boolean.Parse(generateRevisionsForCollectionsStr);

			var ignoreOptimisticLockingPropertyStr = Toolz.GetProperty(properties,
					"envers.do_not_audit_optimistic_locking_field",
					"true");
			DoNotAuditOptimisticLockingField = Boolean.Parse(ignoreOptimisticLockingPropertyStr);

			var storeDataDeletedEntityStr = Toolz.GetProperty(properties,
					"envers.store_data_at_delete",
					"false");
			StoreDataAtDelete = Boolean.Parse(storeDataDeletedEntityStr);

			DefaultSchemaName = Toolz.GetProperty(properties,"envers.default_schema","");
			DefaultCatalogName = Toolz.GetProperty(properties,"envers.default_catalog","");

			//TODO Simon - see if we need to parametrize this, HSQLDialect (HSQLDB) not implemented for NHibernate
			CorrelatedSubqueryOperator = "org.hibernate.dialect.HSQLDialect".Equals(
					Toolz.GetProperty(properties,"hibernate.dialect","")) ? "in" : "=";
		}

		/// <summary>
		/// Should a revision be generated when a not-owned relation field changes
		/// </summary>
		public bool GenerateRevisionsForCollections { get; private set; }

		/// <summary>
		/// Should the optimistic locking property of an entity be considered unversioned
		/// </summary>
		public bool DoNotAuditOptimisticLockingField { get; private set; }

		/// <summary>
		/// Which operator to use in correlated subqueries (when we want a property to be equal to the result of
		/// a correlated subquery, for example: e.p <operator> (select max(e2.p) where e2.p2 = e.p2 ...).
		/// Normally, this should be "=". However, HSQLDB has an issue related to that, so as a workaround,
		/// "in" is used. See {@link org.hibernate.envers.test.various.HsqlTest}.
		/// </summary>
		public string CorrelatedSubqueryOperator { get; private set; }

		/// <summary>
		/// Should entity data be stored when it is deleted
		/// </summary>
		public bool StoreDataAtDelete { get; private set; }

		/// <summary>
		/// The default name of the schema of audit tables.
		/// </summary>
		public string DefaultSchemaName { get; private set; }

		/// <summary>
		/// The default name of the catalog of the audit tables.
		/// </summary>
		public string DefaultCatalogName { get; private set; }
	}
}

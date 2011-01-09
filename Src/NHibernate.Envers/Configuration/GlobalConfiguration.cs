using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org) and Nicolas Doroskevich
     */
    public class GlobalConfiguration {
        // Should a revision be generated when a not-owned relation field changes
        private readonly bool generateRevisionsForCollections;

        // Should the optimistic locking property of an entity be considered unversioned
        private readonly bool doNotAuditOptimisticLockingField;

	    // Should entity data be stored when it is deleted
	    private readonly bool storeDataAtDelete;

        // The default name of the schema of audit tables.
        private readonly String defaultSchemaName;

        // The default name of the catalog of the audit tables.
        private readonly String defaultCatalogName;

        /*
         Which operator to use in correlated subqueries (when we want a property to be equal to the result of
         a correlated subquery, for example: e.p <operator> (select max(e2.p) where e2.p2 = e.p2 ...).
         Normally, this should be "=". However, HSQLDB has an issue related to that, so as a workaround,
         "in" is used. See {@link org.hibernate.envers.test.various.HsqlTest}.
         */
        private readonly String correlatedSubqueryOperator;

        public GlobalConfiguration(IDictionary<string,string> properties) {
            String generateRevisionsForCollectionsStr = Toolz.GetProperty(properties,
                    "NHibernate.Envers.revision_on_collection_change",
                    "NHibernate.Envers.revisionOnCollectionChange",
                    "true");
            generateRevisionsForCollections = Boolean.Parse(generateRevisionsForCollectionsStr);

            String ignoreOptimisticLockingPropertyStr = Toolz.GetProperty(properties,
                    "NHibernate.Envers.do_not_audit_optimistic_locking_field",
                    "NHibernate.Envers.doNotAuditOptimisticLockingField",
                    "true");
            doNotAuditOptimisticLockingField = Boolean.Parse(ignoreOptimisticLockingPropertyStr);

		    String storeDataDeletedEntityStr = Toolz.GetProperty(properties,
                    "NHibernate.Envers.store_data_at_delete",
                    "NHibernate.Envers.storeDataAtDelete",
                    "false");
		    storeDataAtDelete = Boolean.Parse(storeDataDeletedEntityStr);

            defaultSchemaName = Toolz.GetProperty(properties,"NHibernate.Envers.default_schema","");
            defaultCatalogName = Toolz.GetProperty(properties,"NHibernate.Envers.default_catalog","");

            //TODO Simon - see if we need to parametrize this, HSQLDialect (HSQLDB) not implemented for NHibernate
            correlatedSubqueryOperator = "org.hibernate.dialect.HSQLDialect".Equals(
                    Toolz.GetProperty(properties,"hibernate.dialect","")) ? "in" : "=";
        }

        public bool isGenerateRevisionsForCollections() {
            return generateRevisionsForCollections;
        }

        public bool isDoNotAuditOptimisticLockingField() {
            return doNotAuditOptimisticLockingField;
        }

        public String getCorrelatedSubqueryOperator() {
            return correlatedSubqueryOperator;
        }

	    public bool isStoreDataAtDelete() {
		    return storeDataAtDelete;
	    }

        public String getDefaultSchemaName() {
            return defaultSchemaName;
        }

        public String getDefaultCatalogName() {
            return defaultCatalogName;
        }
    }
}

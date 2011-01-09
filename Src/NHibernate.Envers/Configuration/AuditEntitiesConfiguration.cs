using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration
{
    /**
     * Configuration of versions entities - names of fields, entities and tables created to store versioning information.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class AuditEntitiesConfiguration {
        private readonly String auditTablePrefix;
        private readonly String auditTableSuffix;

        public String OriginalIdPropName { get; private set; }

        public String RevisionFieldName { get; private set; }
        public String RevisionNumberPath { get; private set; }
        private readonly String revisionPropBasePath;

        public String RevisionTypePropName { get; private set; }
        public String RevisionTypePropType { get; private set; }

        public String RevisionInfoEntityName { get; private set; }
        /// <summary>
        /// Returns the class name without the assembly name. Used for generating querries
        /// </summary>
        public String RevisionInfoEntityFullClassName {
            get
            {
                return RevisionInfoEntityName.Split(new char[]{','})[0];
            }
        }

        private readonly IDictionary<String, String> customAuditTablesNames;

        public AuditEntitiesConfiguration(IDictionary<String, String> properties, String revisionInfoEntityName)
        {
            this.RevisionInfoEntityName = revisionInfoEntityName;

            auditTablePrefix = Toolz.GetProperty(properties,
                    "NHibernate.envers.audit_table_prefix",
                    "NHibernate.envers.auditTablePrefix",
                    "");
            auditTableSuffix = Toolz.GetProperty(properties,
                    "NHibernate.envers.audit_table_suffix", 
                    "NHibernate.envers.auditTableSuffix",
                    "_AUD");

            OriginalIdPropName = "originalId";

            RevisionFieldName = Toolz.GetProperty(properties,
                    "NHibernate.envers.revision_field_name",
                    "NHibernate.envers.revisionFieldName",
                    "REV");

            RevisionTypePropName = Toolz.GetProperty(properties,
                    "NHibernate.envers.revision_type_field_name", 
                    "NHibernate.envers.revisionTypeFieldName",
                    "REVTYPE");
            RevisionTypePropType = "byte";

            customAuditTablesNames = new Dictionary<String, String>();

            RevisionNumberPath = OriginalIdPropName + "." + RevisionFieldName + ".id";
            revisionPropBasePath = OriginalIdPropName + "." + RevisionFieldName + ".";
        }

        /**
         * @param propertyName Property of the revision entity.
         * @return A path to the given property of the revision entity associated with an audit entity.
         */
        public String GetRevisionPropPath(String propertyName) {
            return revisionPropBasePath + propertyName;
        }

        //

        public void AddCustomAuditTableName(String entityName, String tableName) {
            customAuditTablesNames.Add(entityName, tableName);
        }

        //

        public string GetAuditEntityName(String entityName) 
        {
            return auditTablePrefix + entityName + auditTableSuffix;
        }

        public string GetAuditTableName(string entityName, string tableName) 
        {
            string dicValue;
            if(entityName != null && customAuditTablesNames.TryGetValue(entityName, out dicValue))
                return dicValue;
            return auditTablePrefix + tableName + auditTableSuffix;
        }
    }
}

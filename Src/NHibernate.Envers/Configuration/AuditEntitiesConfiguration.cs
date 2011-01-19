using System;
using System.Collections.Generic;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration
{
	public class AuditEntitiesConfiguration 
	{
		private readonly string auditTablePrefix;
		private readonly string auditTableSuffix;

		public string OriginalIdPropName { get; private set; }

		public string RevisionFieldName { get; private set; }
		public string RevisionNumberPath { get; private set; }
		private readonly string revisionPropBasePath;

		public string RevisionTypePropName { get; private set; }
		public string RevisionTypePropType { get; private set; }

		public string RevisionInfoEntityName { get; private set; }
		/// <summary>
		/// Returns the class name without the assembly name. Used for generating querries
		/// </summary>
		public string RevisionInfoEntityFullClassName 
		{
			get
			{
				return RevisionInfoEntityName.Split(new[]{','})[0];
			}
		}

		private readonly IDictionary<String, String> customAuditTablesNames;

		public AuditEntitiesConfiguration(IDictionary<String, String> properties, string revisionInfoEntityName)
		{
			RevisionInfoEntityName = revisionInfoEntityName;

			auditTablePrefix = Toolz.GetProperty(properties,
					"NHibernate.envers.audit_table_prefix",
					"");
			auditTableSuffix = Toolz.GetProperty(properties,
					"NHibernate.envers.audit_table_suffix",
					"_AUD");

			OriginalIdPropName = "originalId";

			RevisionFieldName = Toolz.GetProperty(properties,
					"NHibernate.envers.revision_field_name",
					"REV");

			RevisionTypePropName = Toolz.GetProperty(properties,
					"NHibernate.envers.revision_type_field_name", 
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
		public string GetRevisionPropPath(string propertyName) {
			return revisionPropBasePath + propertyName;
		}

		//

		public void AddCustomAuditTableName(string entityName, string tableName) {
			customAuditTablesNames.Add(entityName, tableName);
		}

		//

		public string GetAuditEntityName(string entityName) 
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

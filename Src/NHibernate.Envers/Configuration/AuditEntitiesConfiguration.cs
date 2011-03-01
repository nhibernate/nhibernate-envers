using System.Collections.Generic;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration
{
	public class AuditEntitiesConfiguration 
	{
		private readonly string auditTablePrefix;
		private readonly string auditTableSuffix;
		private readonly string revisionPropBasePath;
		private readonly string revInfoEntityName;
		private readonly IDictionary<string, string> customAuditTablesNames;

		public AuditEntitiesConfiguration(IDictionary<string, string> properties, string revisionInfoEntityName)
		{
			revInfoEntityName = revisionInfoEntityName;

			auditTablePrefix = Toolz.GetProperty(properties,
					"NHibernate.envers.audit_table_prefix",
					string.Empty);
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

			customAuditTablesNames = new Dictionary<string, string>();

			RevisionNumberPath = OriginalIdPropName + "." + RevisionFieldName + ".id";
			revisionPropBasePath = OriginalIdPropName + "." + RevisionFieldName + ".";
		}


		public string OriginalIdPropName { get; private set; }
		public string RevisionFieldName { get; private set; }
		public string RevisionNumberPath { get; private set; }
		public string RevisionTypePropName { get; private set; }
		public string RevisionTypePropType { get; private set; }

		/// <summary>
		/// Returns the class name without the assembly name. Used for generating querries
		/// </summary>
		public string RevisionInfoEntityFullClassName 
		{
			get
			{
				return revInfoEntityName.Split(new[]{','})[0];
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyName">Property of the revision entity.</param>
		/// <returns>A path to the given property of the revision entity associated with an audit entity.</returns>
		public string GetRevisionPropPath(string propertyName) 
		{
			return revisionPropBasePath + propertyName;
		}

		public void AddCustomAuditTableName(string entityName, string tableName) 
		{
			customAuditTablesNames.Add(entityName, tableName);
		}

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

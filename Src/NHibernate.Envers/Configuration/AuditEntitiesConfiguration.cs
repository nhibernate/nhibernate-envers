using System.Collections.Generic;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration
{
	public class AuditEntitiesConfiguration 
	{
		private readonly string auditTablePrefix;
		private readonly string auditTableSuffix;
		private readonly string revisionPropBasePath;
		private readonly IDictionary<string, string> customAuditTablesNames;

		public AuditEntitiesConfiguration(IDictionary<string, string> properties, string revisionInfoEntityName)
		{
			RevisionInfoEntityAssemblyQualifiedName = revisionInfoEntityName;

			auditTablePrefix = ConfigurationKey.AuditTablePrefix.PropertyValue(properties);
			auditTableSuffix = ConfigurationKey.AuditTableSuffix.PropertyValue(properties);
			OriginalIdPropName = "originalId";
			RevisionFieldName = ConfigurationKey.RevisionFieldName.PropertyValue(properties);
			RevisionTypePropName = ConfigurationKey.RevisionTypeFieldName.PropertyValue(properties);
			RevisionTypePropType = "byte";

			RevisionEndFieldName = ConfigurationKey.AuditStrategyValidityEndRevFieldName.PropertyValue(properties);
			AuditStrategyType = System.Type.GetType(ConfigurationKey.AuditStrategy.PropertyValue(properties));

			IsRevisionEndTimestampEnabled = bool.Parse(ConfigurationKey.AuditStrategyValidityStoreRevendTimestamp.PropertyValue(properties));
			if (IsRevisionEndTimestampEnabled)
			{
				RevisionEndTimestampFieldName = ConfigurationKey.AuditStrategyValidityRevendTimestampFieldName.PropertyValue(properties);
			}

			customAuditTablesNames = new Dictionary<string, string>();

			RevisionNumberPath = OriginalIdPropName + "." + RevisionFieldName + ".id";
			revisionPropBasePath = OriginalIdPropName + "." + RevisionFieldName + ".";
		}


		public string OriginalIdPropName { get; private set; }
		public string RevisionFieldName { get; private set; }
		public string RevisionNumberPath { get; private set; }
		public string RevisionTypePropName { get; private set; }
		public string RevisionTypePropType { get; private set; }
		public string RevisionEndFieldName { get; private set; }
		public System.Type AuditStrategyType { get; private set; }
		public bool IsRevisionEndTimestampEnabled { get; private set; }
		public string RevisionEndTimestampFieldName { get; private set; }
		public string RevisionInfoEntityAssemblyQualifiedName { get; private set; }

		/// <summary>
		/// Returns the class name without the assembly name. Used for generating querries
		/// </summary>
		public string RevisionInfoEntityFullClassName()
		{
			return RevisionInfoEntityAssemblyQualifiedName.Split(new[] {','})[0];
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

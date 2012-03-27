using System.Collections.Generic;

namespace NHibernate.Envers.Configuration
{
	public class AuditEntitiesConfiguration 
	{
		private readonly string _auditTablePrefix;
		private readonly string _auditTableSuffix;
		private readonly string _revisionPropBasePath;
		private readonly IDictionary<string, string> _customAuditTablesNames;

		public AuditEntitiesConfiguration(IDictionary<string, string> properties, string revisionInfoEntityName)
		{
			RevisionInfoEntityAssemblyQualifiedName = revisionInfoEntityName;

			_auditTablePrefix = ConfigurationKey.AuditTablePrefix.ToString(properties);
			_auditTableSuffix = ConfigurationKey.AuditTableSuffix.ToString(properties);
			OriginalIdPropName = "originalId";
			RevisionFieldName = ConfigurationKey.RevisionFieldName.ToString(properties);
			RevisionTypePropName = ConfigurationKey.RevisionTypeFieldName.ToString(properties);
			RevisionTypePropType = "byte";

			RevisionEndFieldName = ConfigurationKey.AuditStrategyValidityEndRevFieldName.ToString(properties);
			AuditStrategyType = ConfigurationKey.AuditStrategy.ToType(properties);

			IsRevisionEndTimestampEnabled = ConfigurationKey.AuditStrategyValidityStoreRevendTimestamp.ToBool(properties);
			if (IsRevisionEndTimestampEnabled)
			{
				RevisionEndTimestampFieldName = ConfigurationKey.AuditStrategyValidityRevendTimestampFieldName.ToString(properties);
			}

			_customAuditTablesNames = new Dictionary<string, string>();

			RevisionNumberPath = OriginalIdPropName + "." + RevisionFieldName + ".id";
			_revisionPropBasePath = OriginalIdPropName + "." + RevisionFieldName + ".";
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
			return _revisionPropBasePath + propertyName;
		}

		public void AddCustomAuditTableName(string entityName, string tableName) 
		{
			_customAuditTablesNames.Add(entityName, tableName);
		}

		public string GetAuditEntityName(string entityName) 
		{
			return _auditTablePrefix + entityName + _auditTableSuffix;
		}

		public string GetAuditTableName(string entityName, string tableName) 
		{
			string dicValue;
			if(entityName != null && _customAuditTablesNames.TryGetValue(entityName, out dicValue))
				return dicValue;
			return _auditTablePrefix + tableName + _auditTableSuffix;
		}
	}
}

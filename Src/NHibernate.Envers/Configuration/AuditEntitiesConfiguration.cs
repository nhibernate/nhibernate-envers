using System;
using System.Collections.Generic;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration
{
	[Serializable]
	public class AuditEntitiesConfiguration 
	{
		private readonly IEnversNamingStrategy _namingStrategy;
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
			RevisionEndFieldName = ConfigurationKey.AuditStrategyValidityEndRevFieldName.ToString(properties);
			IsRevisionEndTimestampEnabled = ConfigurationKey.AuditStrategyValidityStoreRevendTimestamp.ToBool(properties);
			if (IsRevisionEndTimestampEnabled)
			{
				RevisionEndTimestampFieldName = ConfigurationKey.AuditStrategyValidityRevendTimestampFieldName.ToString(properties);
			}

			_customAuditTablesNames = new Dictionary<string, string>();

			RevisionNumberPath = OriginalIdPropName + "." + RevisionFieldName + ".id";
			_revisionPropBasePath = OriginalIdPropName + "." + RevisionFieldName + ".";
			_namingStrategy = ConfigurationKey.TableNameStrategy.ToInstance<IEnversNamingStrategy>(properties);
			_namingStrategy.Initialize(_auditTablePrefix, _auditTableSuffix);

			EmbeddableSetOrdinalPropertyName = ConfigurationKey.EmbeddableSetOrdinalFieldName.ToString(properties);
		}


		public string OriginalIdPropName { get; private set; }
		public string RevisionFieldName { get; private set; }
		public string RevisionNumberPath { get; private set; }
		public string RevisionTypePropName { get; private set; }
		public string RevisionEndFieldName { get; private set; }
		public bool IsRevisionEndTimestampEnabled { get; private set; }
		public string RevisionEndTimestampFieldName { get; private set; }
		public string RevisionInfoEntityAssemblyQualifiedName { get; private set; }
		public string EmbeddableSetOrdinalPropertyName { get; private set; }

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

		public string GetAuditEntityName(string referencingEntityName, string referencedEntityName)
		{
			return string.Concat(_auditTablePrefix,
							referencingEntityName.Substring(referencingEntityName.LastIndexOf(".") + 1),
							"_",
							referencedEntityName.Substring(referencedEntityName.LastIndexOf(".") + 1),
							_auditTableSuffix);
		}

		public string JoinTableName(Join originalJoinTable)
		{
			return _namingStrategy.JoinTableName(originalJoinTable);
		}

		public string AuditTableName(string entityName, PersistentClass persistentClass)
		{
			string dicValue;
			return _customAuditTablesNames.TryGetValue(entityName, out dicValue) ? 
						dicValue : 
						_namingStrategy.AuditTableName(persistentClass);
		}

		public string UnidirectionOneToManyTableName(PersistentClass referencingPersistentClass, PersistentClass referencedPersistentClass)
		{
			return _namingStrategy.UnidirectionOneToManyTableName(referencingPersistentClass, referencedPersistentClass);
		}

		public string CollectionTableName(Mapping.Collection originalCollection)
		{
			return _namingStrategy.CollectionTableName(originalCollection);
		}
	}
}

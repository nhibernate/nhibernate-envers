using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Event;
using NHibernate.Envers.Strategy;

namespace NHibernate.Envers.Configuration
{
	[Serializable]
	public class GlobalConfiguration 
	{
		public GlobalConfiguration(AuditConfiguration auditConfiguration, IDictionary<string,string> properties) 
		{
			GenerateRevisionsForCollections = ConfigurationKey.RevisionOnCollectionChange.ToBool(properties);
			DoNotAuditOptimisticLockingField = ConfigurationKey.DoNotAuditOptimisticLockingField.ToBool(properties);
			StoreDataAtDelete = ConfigurationKey.StoreDataAtDelete.ToBool(properties);
			IsTrackEntitiesChangedInRevisionEnabled = ConfigurationKey.TrackEntitiesChangedInRevision.ToBool(properties);
			DefaultSchemaName = ConfigurationKey.DefaultSchema.ToString(properties);
			DefaultCatalogName = ConfigurationKey.DefaultCatalog.ToString(properties);
			EnversProxyFactory = ConfigurationKey.ProxyFactory.ToInstance<IEnversProxyFactory>(properties);
			CollectionMapperFactory = ConfigurationKey.CollectionMapperFactory.ToInstance<ICollectionMapperFactory>(properties);
			CorrelatedSubqueryOperator = "=";
			IsGlobalWithModifiedFlag = ConfigurationKey.GlobalWithModifiedFlag.ToBool(properties);
			ModifiedFlagSuffix = ConfigurationKey.ModifiedFlagSuffix.ToString(properties);
			PostInstantiationListener = ConfigurationKey.PostInstantiationListener.ToInstance<IPostInstantiationListener>(properties);
			AuditStrategy = ConfigurationKey.AuditStrategy.ToInstance<IAuditStrategy>(properties);
			AuditStrategy.Initialize(auditConfiguration);
			AllowIdentifierReuse = ConfigurationKey.AllowIdentifierReuse.ToBool(properties);
		}

		/// <summary>
		/// Should a revision be generated when a not-owned relation field changes
		/// </summary>
		public bool GenerateRevisionsForCollections { get; private set; }

		public bool IsTrackEntitiesChangedInRevisionEnabled { get; private set; }

		public void SetTrackEntitiesChangedInRevisionEnabled()
		{
			IsTrackEntitiesChangedInRevisionEnabled = true;
		}

		/// <summary>
		/// Should the optimistic locking property of an entity be considered unversioned
		/// </summary>
		public bool DoNotAuditOptimisticLockingField { get; private set; }

		/// <summary>
		/// Which operator to use in correlated subqueries (when we want a property to be equal to the result of
		/// a correlated subquery).
		/// </summary>
		/// <remarks>
		/// By default the value is "=". However, HSQLDB has an issue related to that, so as a workaround,
		/// "in" is used.
		/// </remarks>
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

		public ICollectionMapperFactory CollectionMapperFactory { get; private set; }
		public IEnversProxyFactory EnversProxyFactory { get; private set; }

		public bool IsGlobalWithModifiedFlag { get; private set; }
		public string ModifiedFlagSuffix { get; private set; }

		public IPostInstantiationListener PostInstantiationListener { get; private set; }

		public IAuditStrategy AuditStrategy { get; private set; }
		public bool AllowIdentifierReuse { get; private set; }
	}
}

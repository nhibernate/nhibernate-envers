using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration
{
	public class GlobalConfiguration 
	{
		public GlobalConfiguration(IDictionary<string,string> properties) 
		{
			var generateRevisionsForCollectionsStr = Toolz.GetProperty(properties, ConfigurationKey.RevisionOnCollectionChange, "true");
			GenerateRevisionsForCollections = Boolean.Parse(generateRevisionsForCollectionsStr);

			var ignoreOptimisticLockingPropertyStr = Toolz.GetProperty(properties, ConfigurationKey.DoNotAuditOptimisticLockingField, "true");
			DoNotAuditOptimisticLockingField = Boolean.Parse(ignoreOptimisticLockingPropertyStr);

			var storeDataDeletedEntityStr = Toolz.GetProperty(properties, ConfigurationKey.StoreDataAtDelete, "false");
			StoreDataAtDelete = Boolean.Parse(storeDataDeletedEntityStr);

			var trackEntityChangesStr = Toolz.GetProperty(properties, ConfigurationKey.TrackEntitiesChangedInRevision, "false");
			IsTrackEntitiesChangedInRevisionEnabled = Boolean.Parse(trackEntityChangesStr);

			DefaultSchemaName = Toolz.GetProperty(properties, ConfigurationKey.DefaultSchema, string.Empty);
			DefaultCatalogName = Toolz.GetProperty(properties, ConfigurationKey.DefaultCatalog, string.Empty);

			var collectionProxyMapperFactoryTypeString = Toolz.GetProperty(properties,
																	 ConfigurationKey.CollectionProxyMapperFactory,
			                                                         "NHibernate.Envers.Configuration.Metadata.DefaultCollectionProxyMapperFactory");
			var collectionProxyMapperFactoryType = System.Type.GetType(collectionProxyMapperFactoryTypeString, true, true);
			CollectionProxyMapperFactory = (ICollectionProxyMapperFactory) Activator.CreateInstance(collectionProxyMapperFactoryType);
			CorrelatedSubqueryOperator = "=";
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

		public ICollectionProxyMapperFactory CollectionProxyMapperFactory { get; private set; }
	}
}

﻿using System.Collections.Generic;
using NHibernate.Envers.Configuration.Metadata;

namespace NHibernate.Envers.Configuration
{
	public class GlobalConfiguration 
	{
		public GlobalConfiguration(IDictionary<string,string> properties) 
		{
			GenerateRevisionsForCollections = ConfigurationKey.RevisionOnCollectionChange.ToBool(properties);
			DoNotAuditOptimisticLockingField = ConfigurationKey.DoNotAuditOptimisticLockingField.ToBool(properties);
			StoreDataAtDelete = ConfigurationKey.StoreDataAtDelete.ToBool(properties);
			IsTrackEntitiesChangedInRevisionEnabled = ConfigurationKey.TrackEntitiesChangedInRevision.ToBool(properties);
			DefaultSchemaName = ConfigurationKey.DefaultSchema.ToString(properties);
			DefaultCatalogName = ConfigurationKey.DefaultCatalog.ToString(properties);
			CollectionProxyMapperFactory = ConfigurationKey.CollectionProxyMapperFactory.ToInstance<ICollectionProxyMapperFactory>(properties);
			CorrelatedSubqueryOperator = "=";
			IsGlobalWithModifiedFlag = ConfigurationKey.GlobalWithModifiedFlag.ToBool(properties);
			ModifiedFlagSuffix = ConfigurationKey.ModifiedFlagSuffix.ToString(properties);
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

		public bool IsGlobalWithModifiedFlag { get; private set; }
		public string ModifiedFlagSuffix { get; private set; }
	}
}

using NHibernate.Cfg;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Strategy;

namespace NHibernate.Envers.Configuration
{
	/// <summary>
	/// Configuration keys for <see cref="Configuration"/>
	/// </summary>
	public static class ConfigurationKey
	{
		/// <summary>
		/// Should Envers track (persist) entity types that have been changed during each revision.
		/// </summary>
		public static readonly ConfigurationEntry<bool> TrackEntitiesChangedInRevision 
			= new ConfigurationEntry<bool>("nhibernate.envers.track_entities_changed_in_revision", "false", toValueString);

		/// <summary>
		/// Should a revision be generated when a not-owned relation
		/// field changes (this can be either a collection in a
		/// one-to-many relation, or the field using "mappedBy"
		/// attribute in a one-to-one relation).
		/// </summary>
		public static readonly ConfigurationEntry<bool> RevisionOnCollectionChange 
			= new ConfigurationEntry<bool>("nhibernate.envers.revision_on_collection_change", "true", toValueString);

		/// <summary>
		/// When true, properties to be used for optimistic locking
		/// will be automatically not audited (their history won't be
		/// stored; it normally doesn't make sense to store it).
		/// </summary>
		public static readonly ConfigurationEntry<bool> DoNotAuditOptimisticLockingField 
			= new ConfigurationEntry<bool>("nhibernate.envers.do_not_audit_optimistic_locking_field", "true", toValueString);

		/// <summary>
		/// Should the entity data be stored in the revision when
		/// the entity is deleted (instead of only storing the id and
		/// all other properties as null). This is normally not
		/// needed, as the data is present in the last-but-one revision.
		/// Sometimes, however, it is easier and more efficient
		/// to access it in the last revision (then the data that
		/// the entity contained before deletion is stored twice).
		/// </summary>
		public static readonly ConfigurationEntry<bool> StoreDataAtDelete 
			= new ConfigurationEntry<bool>("nhibernate.envers.store_data_at_delete", "false", toValueString);

		/// <summary>
		/// The default schema name that should be used for audit
		/// tables. Can be overriden using the <see cref="AuditTableAttribute"/>.
		/// If not present,  the schema will be the same as the schema of the normal
		/// tables.
		/// </summary>
		public static readonly ConfigurationEntry<string> DefaultSchema 
			= new ConfigurationEntry<string>("nhibernate.envers.default_schema", string.Empty, toValueString);

		/// <summary>
		/// The default catalog name that should be used for audit
		/// tables. Can be overriden using the <see cref="AuditTableAttribute"/>.
		/// If not present,  the catalog will be the same as the catalog of the normal
		/// tables.
		/// </summary>
		public static readonly ConfigurationEntry<string> DefaultCatalog 
			= new ConfigurationEntry<string>("nhibernate.envers.default_catalog", string.Empty, toValueString);

		/// <summary>
		/// Responsible to create collection proxies for audited entities.
		/// May be used if NHibernate Core isn't using its
		/// normal types for its mapped collections, eg if a user
		/// defined CollectionTypeFactory is used.
		/// </summary>
		public static readonly ConfigurationEntry<System.Type> CollectionProxyMapperFactory
			= new ConfigurationEntry<System.Type>("nhibernate.envers.collection_proxy_mapper_factory", typeof (DefaultCollectionProxyMapperFactory).AssemblyQualifiedName, toValueString);

		/// <summary>
		/// String that will be appended to the name of an audited
		/// entity to create the name of the entity, that will hold
		/// audit information. If you audit an entity with a table
		/// name Person, in the default setting Envers will generate
		/// a Person_AUD table to store historical data.
		/// </summary>
		public static readonly ConfigurationEntry<string> AuditTablePrefix 
			= new ConfigurationEntry<string>("nhibernate.envers.audit_table_prefix", string.Empty, toValueString);

		/// <summary>
		/// String that will be prepended to the name of an audited
		/// entity to create the name of the entity, that will hold
		/// audit information.
		/// </summary>
		public static readonly ConfigurationEntry<string> AuditTableSuffix 
			= new ConfigurationEntry<string>("nhibernate.envers.audit_table_suffix", "_AUD", toValueString);

		/// <summary>
		/// Name of a field in the audit entity that will hold the revision number.
		/// </summary>
		public static readonly ConfigurationEntry<string> RevisionFieldName 
			= new ConfigurationEntry<string>("nhibernate.envers.revision_field_name", "REV", toValueString);

		/// <summary>
		/// Name of a field in the audit entity that will hold the
		/// type of the revision (currently, this can be: add, mod, del).
		/// </summary>
		public static readonly ConfigurationEntry<string> RevisionTypeFieldName 
			= new ConfigurationEntry<string>("nhibernate.envers.revision_type_field_name", "REVTYPE", toValueString);

		/// <summary>
		/// The audit strategy that should be used when persisting
		/// audit data. The default stores only the revision, at
		/// which an entity was modified. An alternative, the
		/// <see cref="ValidityAuditStrategy"/> stores
		/// both the start revision and the end revision. Together
		/// these define when an audit row was valid, hence the
		/// name ValidityAuditStrategy.
		/// </summary>
		public static readonly ConfigurationEntry<System.Type> AuditStrategy
			= new ConfigurationEntry<System.Type>("nhibernate.envers.audit_strategy", typeof(DefaultAuditStrategy).AssemblyQualifiedName, toValueString);

		/// <summary>
		/// Should the timestamp of the end revision be stored, until
		/// which the data was valid, in addition to the end revision
		/// itself. This is useful to be able to purge old Audit
		/// records out of a relational database by using table partitioning.
		/// Partitioning requires a column that exists within
		/// the table. This property is only evaluated if the
		/// ValidityAuditStrategy is used.
		/// </summary>
		public static readonly ConfigurationEntry<bool> AuditStrategyValidityStoreRevendTimestamp 
			= new ConfigurationEntry<bool>("nhibernate.envers.audit_strategy_validity_store_revend_timestamp", "false", toValueString);

		/// <summary>
		/// The column name that will hold the end revision number
		/// in audit entities. This property is only valid if the
		/// validity audit strategy is used.
		/// </summary>
		public static readonly ConfigurationEntry<string> AuditStrategyValidityEndRevFieldName
			= new ConfigurationEntry<string>("nhibernate.envers.audit_strategy_validity_end_rev_field_name", "REVEND", toValueString);

		/// <summary>
		/// Column name of the timestamp of the end revision until
		/// which the data was valid. Only used if the ValidityAuditStrategy
		/// is used, and <see cref="AuditStrategyValidityStoreRevendTimestamp"/>evaluates to <code>true</code>.
		/// </summary>
		public static readonly ConfigurationEntry<string> AuditStrategyValidityRevendTimestampFieldName 
			= new ConfigurationEntry<string>("nhibernate.envers.audit_strategy_validity_revend_timestamp_field_name", "REVEND_TSTMP", toValueString);

		/// <summary>
		/// Suffix to be used for modified flags columns.
		/// </summary>
		public static readonly ConfigurationEntry<string> ModifiedFlagSuffix
			= new ConfigurationEntry<string>("nhibernate.envers.modified_flag_suffix", "_MOD", toValueString);

		/// <summary>
		/// Should Envers use modified property flags by default
		/// </summary>
		public static readonly ConfigurationEntry<bool> GlobalWithModifiedFlag 
			= new ConfigurationEntry<bool>("nhibernate.envers.global_with_modified_flag", "false", toValueString);

		private static string toValueString(bool userValue)
		{
			return userValue ? "true" : "false";
		}

		private static string toValueString(string userValue)
		{
			return userValue;
		}

		private static string toValueString(System.Type userValue)
		{
			return userValue.AssemblyQualifiedName;
		}
	}
}
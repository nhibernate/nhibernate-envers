using NHibernate.Cfg;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;
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
		public static readonly BoolConfigurationEntry TrackEntitiesChangedInRevision
			= new BoolConfigurationEntry("nhibernate.envers.track_entities_changed_in_revision", "false");

		/// <summary>
		/// Should a revision be generated when a not-owned relation
		/// field changes (this can be either a collection in a
		/// one-to-many relation, or the field using "mappedBy"
		/// attribute in a one-to-one relation).
		/// </summary>
		public static readonly BoolConfigurationEntry RevisionOnCollectionChange
			= new BoolConfigurationEntry("nhibernate.envers.revision_on_collection_change", "true");

		/// <summary>
		/// When true, properties to be used for optimistic locking
		/// will be automatically not audited (their history won't be
		/// stored; it normally doesn't make sense to store it).
		/// </summary>
		public static readonly BoolConfigurationEntry DoNotAuditOptimisticLockingField
			= new BoolConfigurationEntry("nhibernate.envers.do_not_audit_optimistic_locking_field", "true");

		/// <summary>
		/// Should the entity data be stored in the revision when
		/// the entity is deleted (instead of only storing the id and
		/// all other properties as null). This is normally not
		/// needed, as the data is present in the last-but-one revision.
		/// Sometimes, however, it is easier and more efficient
		/// to access it in the last revision (then the data that
		/// the entity contained before deletion is stored twice).
		/// </summary>
		public static readonly BoolConfigurationEntry StoreDataAtDelete
			= new BoolConfigurationEntry("nhibernate.envers.store_data_at_delete", "false");

		/// <summary>
		/// The default schema name that should be used for audit
		/// tables. Can be overriden using the <see cref="AuditTableAttribute"/>.
		/// If not present,  the schema will be the same as the schema of the normal
		/// tables.
		/// </summary>
		public static readonly StringConfigurationEntry DefaultSchema
			= new StringConfigurationEntry("nhibernate.envers.default_schema", string.Empty);

		/// <summary>
		/// The default catalog name that should be used for audit
		/// tables. Can be overriden using the <see cref="AuditTableAttribute"/>.
		/// If not present,  the catalog will be the same as the catalog of the normal
		/// tables.
		/// </summary>
		public static readonly StringConfigurationEntry DefaultCatalog
			= new StringConfigurationEntry("nhibernate.envers.default_catalog", string.Empty);

		/// <summary>
		/// Responsible to create collection mappers (<see cref="IPropertyMapper"/>) for audited entities.
		/// May be used if NHibernate Core isn't using its
		/// normal types for its mapped collections, eg if a user
		/// defined CollectionTypeFactory is used.
		/// </summary>
		public static readonly TypeConfigurationEntry CollectionMapperFactory
			= new TypeConfigurationEntry("nhibernate.envers.collection_mapper_factory", typeof(DefaultCollectionMapperFactory).AssemblyQualifiedName);

		/// <summary>
		/// Resposible to create envers proxies.
		/// </summary>
		public static readonly TypeConfigurationEntry ProxyFactory
			= new TypeConfigurationEntry("nhibernate.envers.proxy_factory", typeof(DefaultEnversProxyFactory).AssemblyQualifiedName);

		/// <summary>
		/// String that will be appended to the name of an audited
		/// entity to create the name of the entity, that will hold
		/// audit information. If you audit an entity with a table
		/// name Person, in the default setting Envers will generate
		/// a Person_AUD table to store historical data.
		/// </summary>
		public static readonly StringConfigurationEntry AuditTablePrefix
			= new StringConfigurationEntry("nhibernate.envers.audit_table_prefix", string.Empty);

		/// <summary>
		/// String that will be prepended to the name of an audited
		/// entity to create the name of the entity, that will hold
		/// audit information.
		/// </summary>
		public static readonly StringConfigurationEntry AuditTableSuffix
			= new StringConfigurationEntry("nhibernate.envers.audit_table_suffix", "_AUD");

		/// <summary>
		/// Name of a field in the audit entity that will hold the revision number.
		/// </summary>
		public static readonly StringConfigurationEntry RevisionFieldName 
			= new StringConfigurationEntry("nhibernate.envers.revision_field_name", "REV");

		/// <summary>
		/// Name of a field in the audit entity that will hold the
		/// type of the revision (currently, this can be: add, mod, del).
		/// </summary>
		public static readonly StringConfigurationEntry RevisionTypeFieldName
			= new StringConfigurationEntry("nhibernate.envers.revision_type_field_name", "REVTYPE");

		/// <summary>
		/// The audit strategy that should be used when persisting
		/// audit data. The default stores only the revision, at
		/// which an entity was modified. An alternative, the
		/// <see cref="ValidityAuditStrategy"/> stores
		/// both the start revision and the end revision. Together
		/// these define when an audit row was valid, hence the
		/// name ValidityAuditStrategy.
		/// </summary>
		public static readonly TypeConfigurationEntry AuditStrategy
			= new TypeConfigurationEntry("nhibernate.envers.audit_strategy", typeof(DefaultAuditStrategy).AssemblyQualifiedName);

		/// <summary>
		/// Should the timestamp of the end revision be stored, until
		/// which the data was valid, in addition to the end revision
		/// itself. This is useful to be able to purge old Audit
		/// records out of a relational database by using table partitioning.
		/// Partitioning requires a column that exists within
		/// the table. This property is only evaluated if the
		/// ValidityAuditStrategy is used.
		/// </summary>
		public static readonly BoolConfigurationEntry AuditStrategyValidityStoreRevendTimestamp
			= new BoolConfigurationEntry("nhibernate.envers.audit_strategy_validity_store_revend_timestamp", "false");

		/// <summary>
		/// The column name that will hold the end revision number
		/// in audit entities. This property is only valid if the
		/// validity audit strategy is used.
		/// </summary>
		public static readonly StringConfigurationEntry AuditStrategyValidityEndRevFieldName
			= new StringConfigurationEntry("nhibernate.envers.audit_strategy_validity_end_rev_field_name", "REVEND");

		/// <summary>
		/// Column name of the timestamp of the end revision until
		/// which the data was valid. Only used if the ValidityAuditStrategy
		/// is used, and <see cref="AuditStrategyValidityStoreRevendTimestamp"/>evaluates to <code>true</code>.
		/// </summary>
		public static readonly StringConfigurationEntry AuditStrategyValidityRevendTimestampFieldName
			= new StringConfigurationEntry("nhibernate.envers.audit_strategy_validity_revend_timestamp_field_name", "REVEND_TSTMP");

		/// <summary>
		/// Suffix to be used for modified flags columns.
		/// </summary>
		public static readonly StringConfigurationEntry ModifiedFlagSuffix
			= new StringConfigurationEntry("nhibernate.envers.modified_flag_suffix", "_MOD");

		/// <summary>
		/// Should Envers use modified property flags by default
		/// </summary>
		public static readonly BoolConfigurationEntry GlobalWithModifiedFlag
			= new BoolConfigurationEntry("nhibernate.envers.global_with_modified_flag", "false");

		/// <summary>
		/// Gives a cfg object a unique name. Used by Envers to use equality on serialized <see cref="Cfg.Configuration"/> objects.
		/// The name will be set first time Envers use this <see cref="Cfg.Configuration"/> object.
		/// </summary>
		public static readonly StringConfigurationEntry UniqueConfigurationName
			= new StringConfigurationEntry("nhibernate.envers.unique_cfg_name", string.Empty);

		/// <summary>
		/// Listener to be invoked after a versioned entity is instantiated. Can be used for DI.
		/// </summary>
		public static readonly TypeConfigurationEntry PostInstantiationListener
			= new TypeConfigurationEntry("nhibernate.envers.post_instantiation_listener", typeof(DefaultEnversPostInstantiationListener).AssemblyQualifiedName);

		/// <summary>
		/// A <see cref="IEnversNamingStrategy"/> giving name to envers auditing tables.
		/// This will override <see cref="AuditTablePrefix"/> and <see cref="AuditTableSuffix"/>.
		/// Setting explicit names to tables will override these default namings.
		/// </summary>
		public static readonly TypeConfigurationEntry TableNameStrategy
			= new TypeConfigurationEntry("nhibernate.envers.table_name_strategy", typeof(DefaultNamingStrategy).AssemblyQualifiedName);

		/// <summary>
		/// Name of column used for storing ordinal of the change in sets of embeddable elements.
		/// </summary>
		public static readonly StringConfigurationEntry EmbeddableSetOrdinalFieldName
			= new StringConfigurationEntry("nhibernate.envers.embeddable_set_ordinal_field_name", "SETORDINAL");

		/// <summary>
		/// Guarantees proper validity audit strategy behavior when application reuses identifiers of deleted entities.
		/// Exactly one row with <code>null</code> end date exists for each identifier.
		/// </summary>
		public static readonly BoolConfigurationEntry AllowIdentifierReuse
			= new BoolConfigurationEntry("nhibernate.envers.allow_identifier_reuse", "false");
	}
}
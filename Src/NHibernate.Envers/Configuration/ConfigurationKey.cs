using NHibernate.Cfg;
using NHibernate.Envers.Configuration.Attributes;
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
		public const string TrackEntitiesChangedInRevision = "nhibernate.envers.track_entities_changed_in_revision";

		/// <summary>
		/// Should a revision be generated when a not-owned relation
		/// field changes (this can be either a collection in a
		/// one-to-many relation, or the field using "mappedBy"
		/// attribute in a one-to-one relation).
		/// </summary>
		public const string RevisionOnCollectionChange = "nhibernate.envers.revision_on_collection_change";

		/// <summary>
		/// When true, properties to be used for optimistic locking
		/// will be automatically not audited (their history won't be
		/// stored; it normally doesn't make sense to store it).
		/// </summary>
		public const string DoNotAuditOptimisticLockingField = "nhibernate.envers.do_not_audit_optimistic_locking_field";

		/// <summary>
		/// Should the entity data be stored in the revision when
		/// the entity is deleted (instead of only storing the id and
		/// all other properties as null). This is normally not
		/// needed, as the data is present in the last-but-one revision.
		/// Sometimes, however, it is easier and more efficient
		/// to access it in the last revision (then the data that
		/// the entity contained before deletion is stored twice).
		/// </summary>
		public const string StoreDataAtDelete = "nhibernate.envers.store_data_at_delete";

		/// <summary>
		/// The default schema name that should be used for audit
		/// tables. Can be overriden using the <see cref="AuditTableAttribute"/>.
		/// If not present,  the schema will be the same as the schema of the normal
		/// tables.
		/// </summary>
		public const string DefaultSchema = "nhibernate.envers.default_schema";

		/// <summary>
		/// The default catalog name that should be used for audit
		/// tables. Can be overriden using the <see cref="AuditTableAttribute"/>.
		/// If not present,  the catalog will be the same as the catalog of the normal
		/// tables.
		/// </summary>
		public const string DefaultCatalog = "nhibernate.envers.default_catalog";

		/// <summary>
		/// Responsible to create collection proxies for audited entities.
		/// May be used if NHibernate Core isn't using its
		/// normal types for its mapped collections, eg if a user
		/// defined CollectionTypeFactory is used.
		/// </summary>
		public const string CollectionProxyMapperFactory = "nhibernate.envers.collection_proxy_mapper_factory";

		/// <summary>
		/// String that will be appended to the name of an audited
		/// entity to create the name of the entity, that will hold
		/// audit information. If you audit an entity with a table
		/// name Person, in the default setting Envers will generate
		/// a Person_AUD table to store historical data.
		/// </summary>
		public const string AuditTablePrefix = "nhibernate.envers.audit_table_prefix";

		/// <summary>
		/// String that will be prepended to the name of an audited
		/// entity to create the name of the entity, that will hold
		/// audit information.
		/// </summary>
		public const string AuditTableSuffix = "nhibernate.envers.audit_table_suffix";

		/// <summary>
		/// Name of a field in the audit entity that will hold the revision number.
		/// </summary>
		public const string RevisionFieldName = "nhibernate.envers.revision_field_name";

		/// <summary>
		/// Name of a field in the audit entity that will hold the
		/// type of the revision (currently, this can be: add, mod, del).
		/// </summary>
		public const string RevisionTypeFieldName = "nhibernate.envers.revision_type_field_name";

		/// <summary>
		/// The audit strategy that should be used when persisting
		/// audit data. The default stores only the revision, at
		/// which an entity was modified. An alternative, the
		/// <see cref="ValidityAuditStrategy"/> stores
		/// both the start revision and the end revision. Together
		/// these define when an audit row was valid, hence the
		/// name ValidityAuditStrategy.
		/// </summary>
		public const string AuditStrategy = "nhibernate.envers.audit_strategy";

		/// <summary>
		/// Should the timestamp of the end revision be stored, until
		/// which the data was valid, in addition to the end revision
		/// itself. This is useful to be able to purge old Audit
		/// records out of a relational database by using table partitioning.
		/// Partitioning requires a column that exists within
		/// the table. This property is only evaluated if the
		/// ValidityAuditStrategy is used.
		/// </summary>
		public const string AuditStrategyValidityStoreRevendTimestamp = "nhibernate.envers.audit_strategy_validity_store_revend_timestamp";

		/// <summary>
		/// The column name that will hold the end revision number
		/// in audit entities. This property is only valid if the
		/// validity audit strategy is used.
		/// </summary>
		public const string AuditStrategyValidityEndRevFieldName = "nhibernate.envers.audit_strategy_validity_end_rev_field_name";

		/// <summary>
		/// Column name of the timestamp of the end revision until
		/// which the data was valid. Only used if the ValidityAuditStrategy
		/// is used, and <see cref="AuditStrategyValidityStoreRevendTimestamp"/>evaluates to <code>true</code>.
		/// </summary>
		public const string AuditStrategyValidityRevendTimestampFieldName = "nhibernate.envers.audit_strategy_validity_revend_timestamp_field_name";

		/// <summary>
		/// Suffix to be used for modified flags columns.
		/// </summary>
		public const string ModifiedFlagSuffix = "nhibernate.envers.modified_flag_suffix";

		/// <summary>
		/// Should Envers use modified property flags by default
		/// </summary>
		public const string GlobalWithModifiedFlag = "nhibernate.envers.global_with_modified_flag";
	}
}
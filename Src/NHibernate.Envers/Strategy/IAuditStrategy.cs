using System.Xml.Linq;
using NHibernate.Collection;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Strategy
{
	/// <summary>
	/// Behaviours of different audit strategy for populating audit data.
	/// </summary>
	public partial interface IAuditStrategy
	{
		/// <summary>
		/// Called once at start up.
		/// </summary>
		void Initialize(AuditConfiguration auditConfiguration);

		/// <summary>
		/// Perform the persistence of audited data for regular entities.
		/// </summary>
		/// <param name="session">Session, which can be used to persist the data.</param>
		/// <param name="entityName">Name of the entity, in which the audited change happens</param>
		/// <param name="id">Id of the entity.</param>
		/// <param name="data">Audit data to persist</param>
		/// <param name="revision">Current revision data</param>
		void Perform(ISession session, string entityName, object id, object data, object revision);

		/// <summary>
		/// Perform the persistence of audited data for collection ("middle") entities.
		/// </summary>
		/// <param name="session">Session, which can be used to persist the data.</param>
		/// <param name="auditCfg">Audit configuration</param>
		/// <param name="persistentCollectionChangeData">Collection change data to be persisted.</param>
		/// <param name="revision">Current revision data</param>
		/// <param name="entityName">Name of the entity, in which the audited change happens.</param>
		/// <param name="propertyName">The name of the property holding the <see cref="IPersistentCollection"/></param>
		void PerformCollectionChange(ISession session, string entityName, string propertyName, AuditConfiguration auditCfg, PersistentCollectionChangeData persistentCollectionChangeData, object revision);

		/// <summary>
		/// Update the rootQueryBuilder with an extra WHERE clause to restrict the revision for a two-entity relation.
		/// This WHERE clause depends on the AuditStrategy, as follows:
		/// <ul>
		/// <li>For {@link DefaultAuditStrategy} a subquery is created: 
		/// <p><code>e.revision = (SELECT max(...) ...)</code></p>
		/// </li>
		/// <li>for {@link ValidityAuditStrategy} the revision-end column is used: 
		/// <p><code>
		/// <![CDATA[e.revision <= :revision and (e.endRevision > :revision or e.endRevision is null)]]>
		/// </code></p>
		/// </li>
		/// </ul>
		/// </summary>
		/// <param name="rootQueryBuilder">The <see cref="QueryBuilder"/> that will be updated</param>
		/// <param name="parameters">Root parameters to which restrictions shall be added</param>
		/// <param name="revisionProperty">Property of the revision column</param>
		/// <param name="revisionEndProperty">Property of the revisionEnd column (only used for <see cref="ValidityAuditStrategy"/>)</param>
		/// <param name="addAlias"><code>bool</code> indicator if a left alias is needed</param>
		/// <param name="idData">Id-information for the two-entity relation (only used for <see cref="DefaultAuditStrategy"/>)</param>
		/// <param name="revisionPropertyPath">Path of the revision property (only used for <see cref="ValidityAuditStrategy"/>)</param>
		/// <param name="originalIdPropertyName">name of the id property (only used for <see cref="ValidityAuditStrategy"/>)</param>
		/// <param name="alias1">alias1 an alias used for subquery (only used for <see cref="ValidityAuditStrategy"/>)</param>
		/// <param name="alias2">alias1 an alias used for subquery (only used for <see cref="ValidityAuditStrategy"/>)</param>
		void AddEntityAtRevisionRestriction(QueryBuilder rootQueryBuilder, Parameters parameters, string revisionProperty,
										string revisionEndProperty, bool addAlias, MiddleIdData idData,
										string revisionPropertyPath, string originalIdPropertyName, string alias1, string alias2);

		/// <summary>
		/// Update the rootQueryBuilder with an extra WHERE clause to restrict the revision for a middle-entity 
		/// association. This WHERE clause depends on the AuditStrategy, as follows:
		/// <ul>
		/// <li>For {@link DefaultAuditStrategy} a subquery is created: 
		/// <p><code>e.revision = (SELECT max(...) ...)</code></p>
		/// </li>
		/// <li>for {@link ValidityAuditStrategy} the revision-end column is used: 
		/// <p><code><![CDATA[e.revision <= :revision and (e.endRevision > :revision or e.endRevision is null)]]></code></p>
		/// </li>
		/// </ul>
		/// </summary>
		/// <param name="rootQueryBuilder">The <see cref="QueryBuilder"/> that will be updated</param>
		/// <param name="parameters">root parameters to which restrictions shall be added</param>
		/// <param name="revisionProperty">Property of the revision column</param>
		/// <param name="revisionEndProperty">Property of the revisionEnd column (only used for <see cref="ValidityAuditStrategy"/>)</param>
		/// <param name="addAlias"><code>bool</code> indicator if a left alias is needed</param>
		/// <param name="referencingIdData">id-information for the middle-entity association (only used for <see cref="DefaultAuditStrategy"/>)</param>
		/// <param name="versionsMiddleEntityName">name of the middle-entity</param>
		/// <param name="eeOriginalIdPropertyPath">name of the id property (only used for <see cref="ValidityAuditStrategy"/>)</param>
		/// <param name="revisionPropertyPath">path of the revision property (only used for <see cref="ValidityAuditStrategy"/>)</param>
		/// <param name="originalIdPropertyName">name of the id property (only used for <see cref="ValidityAuditStrategy"/>)</param>
		/// <param name="alias1">An alias used for subqueries (only used for <see cref="DefaultAuditStrategy"/>)</param>
		/// <param name="inclusive">indicates whether revision number shall be treated as inclusive or exclusive</param>
		/// <param name="componentDatas">information about the middle-entity relation
		/// <remarks>
		/// <code>null</code> is accepted.
		/// </remarks>
		/// </param>
		void AddAssociationAtRevisionRestriction(QueryBuilder rootQueryBuilder, Parameters parameters, string revisionProperty, string revisionEndProperty,
									bool addAlias, MiddleIdData referencingIdData, string versionsMiddleEntityName,
									string eeOriginalIdPropertyPath, string revisionPropertyPath, string originalIdPropertyName, 
									string alias1, bool inclusive, params MiddleComponentData[] componentDatas);

		/// <summary>
		/// Adds extra revision mapping for audited entities.
		/// Can add more columns than mandatory Id (entityId, Ref to revision entity) and Revision type (Add, deleted or updated)
		/// </summary>
		/// <param name="classMapping">The audited class mapping where the extra info should be added.</param>
		/// <param name="revisionInfoRelationMapping"><![CDATA[<many-to-one>]]> mapping to the revision entity.</param>
		void AddExtraRevisionMapping(XElement classMapping, XElement revisionInfoRelationMapping);
	}
}
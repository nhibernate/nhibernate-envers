using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Impl;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query
{
	public class AuditQueryCreator
	{
		private readonly AuditConfiguration auditCfg;
		private readonly IAuditReaderImplementor auditReaderImplementor;

		public AuditQueryCreator(AuditConfiguration auditCfg, IAuditReaderImplementor auditReaderImplementor)
		{
			this.auditCfg = auditCfg;
			this.auditReaderImplementor = auditReaderImplementor;
		}

		/// <summary>
		/// Creates a query, which will return entities satisfying some conditions (specified later), at a given revision.
		/// Deleted entities are not included.
		/// </summary>
		/// <param name="c"><see cref="System.Type"/> of the entities for which to query.</param>
		/// <param name="revision">Revision number at which to execute the query.</param>
		/// <returns>A query for entities at a given revision, to which conditions can be added and which can then be executed</returns>
		/// <remarks>The result of the query will be a list of entities instances, unless a projection is added.</remarks>
		public IAuditQuery ForEntitiesAtRevision(System.Type c, long revision)
		{
			ArgumentsTools.CheckPositive(revision, "revision");
			return new EntitiesAtRevisionQuery(auditCfg, auditReaderImplementor, c, revision, false);
		}

		/// <summary>
		/// Creates a query, which will return entities satisfying some conditions (specified later), at a given revision.
		/// Deleted entities are not included.
		/// </summary>
		/// <param name="entityName">Name of entity</param>
		/// <param name="revision">Revision number at which to execute the query.</param>
		/// <returns>A query for entities at a given revision, to which conditions can be added and which can then be executed</returns>
		/// <remarks>The result of the query will be a list of entities instances, unless a projection is added.</remarks>
		public IAuditQuery ForEntitiesAtRevision(string entityName, long revision)
		{
			ArgumentsTools.CheckPositive(revision, "revision");
			return new EntitiesAtRevisionQuery(auditCfg, auditReaderImplementor, entityName, revision, false);
		}

		/// <summary>
		/// Creates a query, which will return entities satisfying some conditions (specified later), at a given revision.
		/// Deleted entities may be optionally included.
		/// </summary>
		/// <param name="entityName">Name of entity</param>
		/// <param name="revision">Revision number at which to execute the query.</param>
		/// <param name="includeDeletions">Whether to include deleted entities in the search.</param>
		/// <returns>A query for entities at a given revision, to which conditions can be added and which can then be executed</returns>
		/// <remarks>The result of the query will be a list of entities instances, unless a projection is added.</remarks>
		public IAuditQuery ForEntitiesAtRevision(string entityName, long revision, bool includeDeletions)
		{
			ArgumentsTools.CheckPositive(revision, "revision");
			return new EntitiesAtRevisionQuery(auditCfg, auditReaderImplementor, entityName, revision, includeDeletions);
		}

		/// <summary>
		/// Creates a query, which will return entities modified at the specified revision.
		/// In comparison, the <seealso cref="ForEntitiesAtRevision(System.Type, long)"/> query takes into all entities
		/// which were present at a given revision, even if they were not modified.
		/// </summary>
		/// <param name="c">Class of the entities for which to query.</param>
		/// <param name="revision">Revision number at which to execute the query.</param>
		/// <returns>
		/// A query for entities changed at a given revision, to which conditions can be added and which
		/// can then be executed.
		/// </returns>
		public IAuditQuery ForEntitiesModifiedAtRevision(System.Type c, long revision)
		{
			ArgumentsTools.CheckPositive(revision, "revision");
			return new EntitiesModifiedAtRevisionQuery(auditCfg, auditReaderImplementor, c, revision);
		}

		/// <summary>
		/// Creates a query, which will return entities modified at the specified revision.
		/// In comparison, the <seealso cref="ForEntitiesAtRevision(System.Type, long)"/> query takes into all entities
		/// which were present at a given revision, even if they were not modified.
		/// </summary>
		/// <param name="entityName">Name of the entity.</param>
		/// <param name="revision">Revision number at which to execute the query.</param>
		/// <returns>
		/// A query for entities changed at a given revision, to which conditions can be added and which
		/// can then be executed.
		/// </returns>
		public IAuditQuery ForEntitiesModifiedAtRevision(string entityName, long revision)
		{
			ArgumentsTools.CheckPositive(revision, "revision");
			return new EntitiesModifiedAtRevisionQuery(auditCfg, auditReaderImplementor, entityName, revision);
		}

		/// <summary>
		/// Creates a query, which will return entities satisfying some conditions (specified later), at a given revision.
		/// </summary>
		/// <typeparam name="TEntity">The <see cref="System.Type"/> of the entities for which to query.</typeparam>
		/// <param name="revision">Revision number at which to execute the query.</param>
		/// <returns>A query for entities at a given revision, to which conditions can be added and which can then be executed</returns>
		public IEntityAuditQuery<TEntity> ForEntitiesAtRevision<TEntity>(long revision) where TEntity : class
		{
			ArgumentsTools.CheckPositive(revision, "Entity revision");
			return new AllEntitiesAtRevisionQuery<TEntity>(auditCfg, auditReaderImplementor, revision);
		}

		/// <summary>
		/// Creates a query, which selects the revisions, at which the given entity was modified.
		/// Unless an explicit projection is set, the result will be a list of three-element arrays, containing:
		/// <ol>
		/// <li>the entity instance</li>
		/// <li>revision entity, corresponding to the revision at which the entity was modified. If no custom
		/// revision entity is used, this will be an instance of <see cref="DefaultRevisionEntity"/></li>
		/// <li>type of the revision (an enum instance of class <see cref="RevisionType"/></li>.
		/// </ol>
		/// Additional conditions that the results must satisfy may be specified. 
		/// </summary>
		/// <param name="c">Class of the entities for which to query.</param>
		/// <param name="selectEntitiesOnly">
		/// If true, instead of a list of three-element arrays, a list of entites will be returned as a result of executing this query.
		/// </param>
		/// <param name="selectDeletedEntities">
		/// If true, also revisions where entities were deleted will be returned. 
		/// The additional entities will have revision type "delete", and contain no data (all fields null), except for the id field.
		/// </param>
		/// <returns>
		/// A query for revisions at which instances of the given entity were modified, to which
		/// conditions can be added (for example - a specific id of an entity of class <code>c</code>), and which
		/// can then be executed. The results of the query will be sorted in ascending order by the revision number,
		/// unless an order or projection is added.
		/// </returns>
		public IAuditQuery ForRevisionsOfEntity(System.Type c, bool selectEntitiesOnly, bool selectDeletedEntities)
		{
			return new RevisionsOfEntityQuery(auditCfg, auditReaderImplementor, c, selectEntitiesOnly, selectDeletedEntities);
		}

		/// <summary>
		/// Creates a query, which selects the revisions, at which the given entity was modified.
		/// Unless an explicit projection is set, the result will be a list of three-element arrays, containing:
		/// <ol>
		/// <li>the entity instance</li>
		/// <li>revision entity, corresponding to the revision at which the entity was modified. If no custom
		/// revision entity is used, this will be an instance of <see cref="DefaultRevisionEntity"/></li>
		/// <li>type of the revision (an enum instance of class <see cref="RevisionType"/></li>.
		/// </ol>
		/// Additional conditions that the results must satisfy may be specified. 
		/// </summary>
		/// <param name="entityName">Name of the entity</param>
		/// <param name="selectEntitiesOnly">
		/// If true, instead of a list of three-element arrays, a list of entites will be returned as a result of executing this query.
		/// </param>
		/// <param name="selectDeletedEntities">
		/// If true, also revisions where entities were deleted will be returned. 
		/// The additional entities will have revision type "delete", and contain no data (all fields null), except for the id field.
		/// </param>
		/// <returns>
		/// A query for revisions at which instances of the given entity were modified, to which
		/// conditions can be added (for example - a specific id of an entity of class <code>c</code>), and which
		/// can then be executed. The results of the query will be sorted in ascending order by the revision number,
		/// unless an order or projection is added.
		/// </returns>
		public IAuditQuery ForRevisionsOfEntity(string entityName, bool selectEntitiesOnly, bool selectDeletedEntities)
		{
			return new RevisionsOfEntityQuery(auditCfg, auditReaderImplementor, entityName, selectEntitiesOnly, selectDeletedEntities);
		}

		/// <summary>
		/// Creates a query, which selects the revisions, at which the given entity was modified.
		/// </summary>
		/// <typeparam name="TEntity">The <see cref="System.Type"/> of the entities for which to query.</typeparam>
		/// <returns>List of <typeparamref name="TEntity"/> instances of each revision excluding deletation.</returns>
		/// <remarks>The results of the query will be sorted in ascending order by the revision number, unless an order or projection is added.</remarks>
		public IEntityAuditQuery<TEntity> ForRevisionsOf<TEntity>() where TEntity : class
		{
			return ForRevisionsOf<TEntity>(false);
		}

		/// <summary>
		/// Creates a query, which selects the revisions, at which the given entity was modified.
		/// </summary>
		/// <typeparam name="TEntity">The <see cref="System.Type"/> of the entities for which to query.</typeparam>
		/// <param name="includesDeleted">If true, also revisions where entities were deleted will be returned.
		/// <remarks>
		/// The additional entities will have revision type <see cref="RevisionType.Deleted"/>, and contain no data (all fields null), except for the id field.
		/// </remarks>
		/// </param>
		/// <remarks>The results of the query will be sorted in ascending order by the revision number, unless an order or projection is added.</remarks>
		public IEntityAuditQuery<TEntity> ForRevisionsOf<TEntity>(bool includesDeleted) where TEntity : class
		{
			return new RevisionsQuery<TEntity>(auditCfg, auditReaderImplementor, includesDeleted);
		}

		/// <summary>
		/// Creates a query, which selects the revisions, at which the given entity was modified.
		/// </summary>
		/// <typeparam name="TEntity">The <see cref="System.Type"/> of the entities for which to query.</typeparam>
		/// <typeparam name="TRevisionEntity">The <see cref="System.Type"/> of the revision entity</typeparam>
		/// <returns>
		/// A query for revisions at which instances of the given entity were modified (including deletation), to which
		/// conditions can be added (for example - a specific id of the entity) and which can then be executed.
		/// The results of the query will be sorted in ascending order by the revision number,
		/// unless an order or projection is added.		
		/// </returns>
		public IEntityAuditQuery<IRevisionEntityInfo<TEntity, TRevisionEntity>> ForHistoryOf<TEntity, TRevisionEntity>()
			where TEntity : class
			where TRevisionEntity : class
		{
			return ForHistoryOf<TEntity, TRevisionEntity>(true);
		}

		/// <summary>
		/// Creates a query, which selects the revisions, at which the given entity was modified.
		/// </summary>
		/// <typeparam name="TEntity">The <see cref="System.Type"/> of the entities for which to query.</typeparam>
		/// <typeparam name="TRevisionEntity">The <see cref="System.Type"/> of the revision entity</typeparam>
		/// <param name="includeDeleted">If true, also revisions where entities were deleted will be returned.</param> 
		/// <returns>
		/// A query for revisions at which instances of the given entity were modified, to which
		/// conditions can be added (for example - a specific id of the entity) and which can then be executed.
		/// The results of the query will be sorted in ascending order by the revision number,
		/// unless an order or projection is added.		
		/// </returns>
		public IEntityAuditQuery<IRevisionEntityInfo<TEntity, TRevisionEntity>> ForHistoryOf<TEntity, TRevisionEntity>(bool includeDeleted)
			where TEntity : class
			where TRevisionEntity : class
		{
			return new HistoryQuery<TEntity, TRevisionEntity>(auditCfg, auditReaderImplementor, includeDeleted);
		}
	}
}
using System;
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
		/// </summary>
		/// <param name="c"><see cref="System.Type"/> of the entities for which to query.</param>
		/// <param name="revision">Revision number at which to execute the query.</param>
		/// <returns>A query for entities at a given revision, to which conditions can be added and which can then be executed</returns>
		/// <remarks>The result of the query will be a list of entities instances, unless a projection is added.</remarks>
		public IAuditQuery ForEntitiesAtRevision(System.Type c, long revision)
		{
			//throw new NotImplementedException("Query not implemented yet");
			ArgumentsTools.CheckNotNull(revision, "Entity revision");
			ArgumentsTools.CheckPositive(revision, "Entity revision");
			return new EntitiesAtRevisionQuery(auditCfg, auditReaderImplementor, c, revision);
		}

		/**
         * Creates a query, which selects the revisions, at which the given entity was modified.
         * Unless an explicit projection is set, the result will be a list of three-element arrays, containing:
         * <ol>
         * <li>the entity instance</li>
         * <li>revision entity, corresponding to the revision at which the entity was modified. If no custom
         * revision entity is used, this will be an instance of {@link org.hibernate.envers.DefaultRevisionEntity}</li>
         * <li>type of the revision (an enum instance of class {@link org.hibernate.envers.RevisionType})</li>.
         * </ol>
         * Additional conditions that the results must satisfy may be specified.
         * @param c Class of the entities for which to query.
         * @param selectEntitiesOnly If true, instead of a list of three-element arrays, a list of entites will be
         * returned as a result of executing this query.
         * @param selectDeletedEntities If true, also revisions where entities were deleted will be returned. The additional
         * entities will have revision type "delete", and contain no data (all fields null), except for the id field.
         * @return A query for revisions at which instances of the given entity were modified, to which
         * conditions can be added (for example - a specific id of an entity of class <code>c</code>), and which
         * can then be executed. The results of the query will be sorted in ascending order by the revision number,
         * unless an order or projection is added.
         */
		public IAuditQuery ForRevisionsOfEntity(System.Type c, bool selectEntitiesOnly, bool selectDeletedEntities)
		{
			return new RevisionsOfEntityQuery(auditCfg, auditReaderImplementor, c, selectEntitiesOnly, selectDeletedEntities);
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
		/// <param name="includesDeletations">If true, also revisions where entities were deleted will be returned. 
		/// <remarks>
		/// The additional entities will have revision type <see cref="RevisionType.Deleted"/>, and contain no data (all fields null), except for the id field.
		/// </remarks>
		/// </param>
		/// <remarks>The results of the query will be sorted in ascending order by the revision number, unless an order or projection is added.</remarks>
		public IEntityAuditQuery<TEntity> ForRevisionsOf<TEntity>(bool includesDeletations) where TEntity : class
		{
			return new RevisionsQuery<TEntity>(auditCfg, auditReaderImplementor, includesDeletations);
		}

		public IRevisionEntityInfo<TEntity, DefaultRevisionEntity> ForHistoryOf<TEntity>() where TEntity : class
		{
			return ForHistoryOf<TEntity>(true);
		}

		public IRevisionEntityInfo<TEntity, DefaultRevisionEntity> ForHistoryOf<TEntity>(bool selectDeletedEntities) where TEntity : class
		{
			//			return new RevisionsOfEntityQuery(auditCfg, auditReaderImplementor, typeof(TEntity), false, selectDeletedEntities);
			throw new NotImplementedException();
		}
	}
}
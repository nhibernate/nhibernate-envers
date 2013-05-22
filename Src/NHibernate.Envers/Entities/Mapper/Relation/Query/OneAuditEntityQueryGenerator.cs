using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from an audit entity.
	/// 
	/// Used when bidirectional onetomany mapping of entities is used.
	/// </summary>
	[Serializable]
	public sealed class OneAuditEntityQueryGenerator: AbstractRelationQueryGenerator 
	{
		private readonly string _queryString;
		private readonly string _queryRemovedString;

		public OneAuditEntityQueryGenerator(AuditEntitiesConfiguration verEntCfg,
											IAuditStrategy auditStrategy,
											MiddleIdData referencingIdData, 
											string referencedEntityName,
											MiddleIdData referencedIdData,
											bool revisionTypeInId) 
			:base(verEntCfg, referencingIdData, revisionTypeInId)
		{
			/*
			 * The valid query that we need to create:
			 *   SELECT new list(e) FROM versionsReferencedEntity e
			 *   WHERE
			 * (only entities referenced by the association; id_ref_ing = id of the referencing entity)
			 *     e.id_ref_ing = :id_ref_ing AND
			 * (selecting e entities at revision :revision)
			 *		--> for DefaultAuditStrategy:
			 *			e.revision = (SELECT max(e2.revision) FROM versionsReferencedEntity e2
			 *			WHERE e2.revision <= :revision AND e2.id = e.id)
			 *		--> for ValidityAuditStrategy
			 *			e.revision <= :revision and (e.endRevision > :revision or e.endRevision is null)
			 *	AND
			 * (only non-deleted entities)
			 *     e.revision_type != DEL
			 */
			var commonPart = commonQueryPart(verEntCfg.GetAuditEntityName(referencedEntityName));
			var validQuery = (QueryBuilder)commonPart.Clone();
			var removedQuery = (QueryBuilder)commonPart.Clone();
			
			createValidDataRestrictions(auditStrategy, referencedIdData, validQuery, validQuery.RootParameters);
			createValidAndRemovedDataRestrictions(auditStrategy, referencedIdData, removedQuery);

			_queryString = QueryToString(validQuery);
			_queryRemovedString = QueryToString(removedQuery);
		}

		/// <summary>
		/// Compute common part for both queries.
		/// </summary>
		private QueryBuilder commonQueryPart(string versionsReferencedEntityName)
		{
			// SELECT e FROM versionsEntity e
			var qb = new QueryBuilder(versionsReferencedEntityName, QueryConstants.ReferencedEntityAlias);
			qb.AddProjection(null, QueryConstants.ReferencedEntityAlias, false, false); 
			//WHERE
			// e.id_ref_ed = :id_ref_ed
			ReferencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(qb.RootParameters, null, true);
			return qb;
		}

		/// <summary>
		/// Creates query restrictions used to retrieve only actual data.
		/// </summary>
		private void createValidDataRestrictions(IAuditStrategy auditStrategy,
		                                         MiddleIdData referencedIdData, QueryBuilder qb, Parameters rootParameters)
		{
			var revisionPropertyPath = VerEntCfg.RevisionNumberPath;
			// (selecting e entities at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddEntityAtRevisionRestriction(qb, rootParameters, revisionPropertyPath, VerEntCfg.RevisionEndFieldName, true,
				referencedIdData, revisionPropertyPath, VerEntCfg.OriginalIdPropName, QueryConstants.ReferencedEntityAlias, QueryConstants.ReferencedEntityAliasDefAudStr);
			// e.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(RevisionTypePath(), false, "!=", QueryConstants.DelRevisionTypeParameter);
		}

		/// <summary>
		/// Create query restrictions used to retrieve actual data and deletions that took place at exactly given revision.
		/// </summary>
		private void createValidAndRemovedDataRestrictions(IAuditStrategy auditStrategy, MiddleIdData referencedIdData, QueryBuilder remQb)
		{
			var disjoint = remQb.RootParameters.AddSubParameters("or");
			var valid = disjoint.AddSubParameters("and");// Restrictions to match all valid rows.
			var removed = disjoint.AddSubParameters("and");// Restrictions to match all rows deleted at exactly given revision.
			// Excluding current revision, because we need to match data valid at the previous one.
			createValidDataRestrictions(auditStrategy, referencedIdData, remQb, valid);
			// e.revision = :revision
			removed.AddWhereWithNamedParam(VerEntCfg.RevisionNumberPath, false, "=", QueryConstants.RevisionParameter);
			// e.revision_type = DEL
			removed.AddWhereWithNamedParam(RevisionTypePath(), false, "=", QueryConstants.DelRevisionTypeParameter);
		}

		protected override bool TransformResultToList()
		{
			return true;
		}

		protected override string QueryString()
		{
			return _queryString;
		}

		protected override string QueryRemovedString()
		{
			return _queryRemovedString;
		}
	}
}
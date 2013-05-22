using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from a relation middle-table only.
	/// 
	/// Used when onetomany mapping of components or primitives is used.
	/// </summary>
	[Serializable]
	public sealed class OneEntityQueryGenerator : AbstractRelationQueryGenerator 
	{
		private readonly string _queryString;
		private readonly string _queryRemovedString;

		public OneEntityQueryGenerator(AuditEntitiesConfiguration verEntCfg,
										IAuditStrategy auditStrategy,
										string versionsMiddleEntityName,
										MiddleIdData referencingIdData,
										bool revisionTypeInId,
										IEnumerable<MiddleComponentData> componentDatas) 
			: base(verEntCfg, referencingIdData, revisionTypeInId)
		{
			/*
			 * The query that we need to create:
			 *   SELECT new list(ee) FROM middleEntity ee WHERE
			 * (only entities referenced by the association; id_ref_ing = id of the referencing entity)
			 *     ee.originalId.id_ref_ing = :id_ref_ing AND
			 * (the association at revision :revision)
			 *	--> for DefaultAuditStrategy:
			 *		ee.revision = (SELECT max(ee2.revision) FROM middleEntity ee2
			 *       WHERE ee2.revision <= :revision AND ee2.originalId.* = ee.originalId.*) 
			 *  --> for ValidityAuditStrategy
			 *		ee.revision <= :revision and (ee.endRevision > :revision or ee.endRevision is null)
			 *	AND
			 * (only non-deleted entities and associations)
			 *     ee.revision_type != DEL
			 */
			var commonPart = commonQueryPart(versionsMiddleEntityName);
			var validQuery = (QueryBuilder)commonPart.Clone();
			var removedQuery = (QueryBuilder)commonPart.Clone();

			createValidDataRestrictions(auditStrategy, versionsMiddleEntityName, validQuery, validQuery.RootParameters, true, componentDatas);
			createValidAndRemovedDataRestrictions(auditStrategy, versionsMiddleEntityName, removedQuery, componentDatas);

			_queryString = QueryToString(validQuery);
			_queryRemovedString = QueryToString(removedQuery);
		}

		/// <summary>
		/// Compute common part for both queries.
		/// </summary>
		private QueryBuilder commonQueryPart(string versionsMiddleEntityName)
		{
			// SELECT ee FROM middleEntity ee
			var qb = new QueryBuilder(versionsMiddleEntityName, QueryConstants.MiddleEntityAlias);
			qb.AddProjection("new list", QueryConstants.MiddleEntityAlias, false, false);
			// WHERE
			// ee.originalId.id_ref_ing = :id_ref_ing
			ReferencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(qb.RootParameters, VerEntCfg.OriginalIdPropName, true);
			return qb;
		}

		/// <summary>
		/// Creates query restrictions used to retrieve only actual data.
		/// </summary>
		private void createValidDataRestrictions(IAuditStrategy auditStrategy, string versionsMiddleEntityName,
		                                         QueryBuilder qb, Parameters rootParameters, bool inclusive,
		                                         IEnumerable<MiddleComponentData> componentData)
		{
			var revisionPropertyPath = VerEntCfg.RevisionNumberPath;
			var originalIdPropertyName = VerEntCfg.OriginalIdPropName;
			var eeOriginalIdPropertyPath = QueryConstants.MiddleEntityAlias + "." + originalIdPropertyName;
			// (with ee association at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddAssociationAtRevisionRestriction(qb, rootParameters, revisionPropertyPath,
				VerEntCfg.RevisionEndFieldName, true, ReferencingIdData, versionsMiddleEntityName,
				eeOriginalIdPropertyPath, revisionPropertyPath, originalIdPropertyName, QueryConstants.MiddleEntityAlias, inclusive, componentData.ToArray());
			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(RevisionTypePath(), "!=", QueryConstants.DelRevisionTypeParameter);
		}

		private void createValidAndRemovedDataRestrictions(IAuditStrategy auditStrategy, string versionsMiddleEntityName,
		                                                   QueryBuilder remQb, IEnumerable<MiddleComponentData> componentData)
		{
			var disjoint = remQb.RootParameters.AddSubParameters("or");
			var valid = disjoint.AddSubParameters("and"); // Restrictions to match all valid rows.
			var removed = disjoint.AddSubParameters("and"); // Restrictions to match all rows deleted at exactly given revision.
			// Excluding current revision, because we need to match data valid at the previous one.
			createValidDataRestrictions(auditStrategy, versionsMiddleEntityName, remQb, valid, false, componentData);
			// ee.revision = :revision
			removed.AddWhereWithNamedParam(VerEntCfg.RevisionNumberPath, "=", QueryConstants.RevisionParameter);
			// ee.revision_type = DEL
			removed.AddWhereWithNamedParam(RevisionTypePath(), "=", QueryConstants.DelRevisionTypeParameter);
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

using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from a relation middle-table and a related non-audited entity.
	///
	/// Used when a unidirectional onetomany mapping is used and child side is not audited.
	/// </summary>
	[Serializable]
	public sealed class TwoEntityOneAuditedQueryGenerator : AbstractRelationQueryGenerator
	{
		private readonly string _queryString;
		private readonly string _queryRemovedString;

		public TwoEntityOneAuditedQueryGenerator(AuditEntitiesConfiguration verEntCfg,
										IAuditStrategy auditStrategy,
										string versionsMiddleEntityName,
										MiddleIdData referencingIdData,
										MiddleIdData referencedIdData,
										bool revisionTypeInId,
										IEnumerable<MiddleComponentData> componentDatas)
			: base(verEntCfg, referencingIdData, revisionTypeInId)
		{
			/*
			 * The query that we need to create:
			 *   SELECT new list(ee, e) FROM referencedEntity e, middleEntity ee
			 *   WHERE
			 * (entities referenced by the middle table; id_ref_ed = id of the referenced entity)
			 *     ee.id_ref_ed = e.id_ref_ed AND
			 * (only entities referenced by the association; id_ref_ing = id of the referencing entity)
			 *     ee.id_ref_ing = :id_ref_ing AND
			 *     
			 * (the association at revision :revision)
			 *	--> For DefaultAuditStrategy:
			 *		ee.revision = (SELECT max(ee2.revision) FROM middleEntity ee2
			 *			WHERE ee2.revision <= :revision AND ee2.originalId.* = ee.originalId.*) 
			 *	--> for ValidityAuditStrategy:      
			 *		ee.revision <= :revision and (ee.endRevision > :revision or ee.endRevision is null)
			 * 
			 * AND
			 *       
			 * (only non-deleted entities and associations)
			 *     ee.revision_type != DEL
			 */
			var commonPart = commonQueryPart(referencedIdData, versionsMiddleEntityName, verEntCfg.OriginalIdPropName);
			var validQuery = (QueryBuilder)commonPart.Clone();
			var removedQuery = (QueryBuilder)commonPart.Clone();

			createValidDataRestrictions(auditStrategy, versionsMiddleEntityName, validQuery, validQuery.RootParameters, componentDatas);
			createValidAndRemovedDataRestrictions(auditStrategy, versionsMiddleEntityName, removedQuery, componentDatas);

			_queryString = QueryToString(validQuery);
			_queryRemovedString = QueryToString(removedQuery);
		}

		private QueryBuilder commonQueryPart(MiddleIdData referencedIdData, string versionsMiddleEntityName, string originalIdPropertyName)
		{
			var eeOriginalIdPropertyPath = QueryConstants.MiddleEntityAlias + "." + originalIdPropertyName;
			// SELECT new list(ee) FROM middleEntity ee
			var qb = new QueryBuilder(versionsMiddleEntityName, QueryConstants.MiddleEntityAlias);
			qb.AddFrom(referencedIdData.EntityName, QueryConstants.ReferencedEntityAlias);
			qb.AddProjection("new list", QueryConstants.MiddleEntityAlias + ", " + QueryConstants.ReferencedEntityAlias, false, false);
			// WHERE
			var rootParameters = qb.RootParameters;
			// ee.id_ref_ed = e.id_ref_ed
			referencedIdData.PrefixedMapper.AddIdsEqualToQuery(rootParameters, eeOriginalIdPropertyPath,
					referencedIdData.OriginalMapper, QueryConstants.ReferencedEntityAlias);
			// ee.originalId.id_ref_ing = :id_ref_ing
			ReferencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);
			return qb;
		}

		private void createValidDataRestrictions(IAuditStrategy auditStrategy, String versionsMiddleEntityName,
		                                         QueryBuilder qb, Parameters rootParameters, IEnumerable<MiddleComponentData> componentData)
		{
			var revisionPropertyPath = VerEntCfg.RevisionNumberPath;
			var originalIdPropertyName = VerEntCfg.OriginalIdPropName;
			var eeOriginalIdPropertyPath = QueryConstants.MiddleEntityAlias + "." + originalIdPropertyName;
			// (with ee association at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddAssociationAtRevisionRestriction(qb, rootParameters, revisionPropertyPath,
							VerEntCfg.RevisionEndFieldName, true, ReferencingIdData, versionsMiddleEntityName,
							eeOriginalIdPropertyPath, revisionPropertyPath, originalIdPropertyName, QueryConstants.MiddleEntityAlias, true, componentData.ToArray());

			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(RevisionTypePath(), "!=", QueryConstants.DelRevisionTypeParameter);
		}

		private void createValidAndRemovedDataRestrictions(IAuditStrategy auditStrategy, String versionsMiddleEntityName,
		                                                   QueryBuilder remQb, IEnumerable<MiddleComponentData> componentData)
		{
			var disjoint = remQb.RootParameters.AddSubParameters("or");
			var valid = disjoint.AddSubParameters("and"); // Restrictions to match all valid rows.
			var removed = disjoint.AddSubParameters("and"); // Restrictions to match all rows deleted at exactly given revision.
			createValidDataRestrictions(auditStrategy, versionsMiddleEntityName, remQb, valid, componentData);
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

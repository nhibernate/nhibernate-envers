using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from a relation middle-table and a related versions entity.
	/// 
	/// Used when a unidirectional onetomany mapping is used and child side is audited.
	/// </summary>
	[Serializable]
	public sealed class TwoEntityQueryGenerator : AbstractRelationQueryGenerator
	{
		private readonly string _queryString;
		private readonly string _queryRemovedString;

		public TwoEntityQueryGenerator(AuditEntitiesConfiguration verEntCfg,
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
			 *   SELECT new list(ee, e) FROM versionsReferencedEntity e, middleEntity ee
			 *   WHERE
			 * (entities referenced by the middle table; id_ref_ed = id of the referenced entity)         
			 *     ee.id_ref_ed = e.id_ref_ed AND
			 * (only entities referenced by the association; id_ref_ing = id of the referencing entity)
			 *     ee.id_ref_ing = :id_ref_ing AND
			 *     
			 * (selecting e entities at revision :revision)
			 *	--> for DefaultAuditStrategy:
			 *		e.revision = (SELECT max(e2.revision) FROM versionsReferencedEntity e2
			 *			WHERE e2.revision <= :revision AND e2.id = e.id)
			 *	--> for ValidTimeAuditStrategy:
			 *		e.revision <= :revision and (e.endRevision > :revision or e.endRevision is null)
			 *       
			 * (the association at revision :revision)
			 *	--> for DefaultAuditStrategy:
			 *		ee.revision = (SELECT max(ee2.revision) FROM middleEntity ee2
			 *			WHERE ee2.revision <= :revision AND ee2.originalId.* = ee.originalId.*) 
			 *	--> for ValidityAuditStrategy:
			 *		ee.revision <= :revision and (ee.endRevision > :revision or ee.endRevision is null)
			 *		
			 * AND
			 *       
			 * (only non-deleted entities and associations)
			 *     ee.revision_type != DEL AND
			 *     e.revision_type != DEL
			 */
			var commonPart = commonQueryPart(referencedIdData, versionsMiddleEntityName, verEntCfg.OriginalIdPropName);

			var validQuery = (QueryBuilder)commonPart.Clone();
			var removedQuery = (QueryBuilder)commonPart.Clone();
			createValidDataRestrictions(auditStrategy, referencedIdData, versionsMiddleEntityName, validQuery, validQuery.RootParameters, true, componentDatas);
			createValidAndRemovedDataRestrictions(auditStrategy, referencedIdData, versionsMiddleEntityName, removedQuery, componentDatas);

			_queryString = QueryToString(validQuery);
			_queryRemovedString = QueryToString(removedQuery);
		}

		private QueryBuilder commonQueryPart(MiddleIdData referencedIdData, string versionsMiddleEntityName,
																				 string originalIdPropertyName)
		{
			var eeOriginalIdPropertyPath = QueryConstants.MiddleEntityAlias + "." + originalIdPropertyName;
			// SELECT new list(ee) FROM middleEntity ee
			var qb = new QueryBuilder(versionsMiddleEntityName, QueryConstants.MiddleEntityAlias);
			qb.AddFrom(referencedIdData.AuditEntityName, QueryConstants.ReferencedEntityAlias);
			qb.AddProjection("new list", QueryConstants.MiddleEntityAlias + ", " + QueryConstants.ReferencedEntityAlias, false, false);
			// WHERE
			var rootParameters = qb.RootParameters;
			// ee.id_ref_ed = e.id_ref_ed
			referencedIdData.PrefixedMapper.AddIdsEqualToQuery(rootParameters, eeOriginalIdPropertyPath,
					referencedIdData.OriginalMapper, QueryConstants.ReferencedEntityAlias + "." + originalIdPropertyName);
			// ee.originalId.id_ref_ing = :id_ref_ing
			ReferencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);
			return qb;
		}

		private void createValidDataRestrictions(IAuditStrategy auditStrategy, MiddleIdData referencedIdData,
																						 String versionsMiddleEntityName, QueryBuilder qb, Parameters rootParameters,
																						 bool inclusive, IEnumerable<MiddleComponentData> componentData)
		{
			var revisionPropertyPath = VerEntCfg.RevisionNumberPath;
			var originalIdPropertyName = VerEntCfg.OriginalIdPropName;
			var eeOriginalIdPropertyPath = QueryConstants.MiddleEntityAlias + "." + originalIdPropertyName;
			var revisionTypePropName = RevisionTypePath();
			// (selecting e entities at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddEntityAtRevisionRestriction(qb, rootParameters,
																									 QueryConstants.ReferencedEntityAlias + "." + revisionPropertyPath,
																									 QueryConstants.ReferencedEntityAlias + "." +
																									 VerEntCfg.RevisionEndFieldName, false,
																									 referencedIdData, revisionPropertyPath, originalIdPropertyName,
																									 QueryConstants.ReferencedEntityAlias,
																									 QueryConstants.ReferencedEntityAliasDefAudStr);

			// (with ee association at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddAssociationAtRevisionRestriction(qb, rootParameters, revisionPropertyPath,
																												VerEntCfg.RevisionEndFieldName, true, ReferencingIdData,
																												versionsMiddleEntityName,
																												eeOriginalIdPropertyPath, revisionPropertyPath,
																												originalIdPropertyName, QueryConstants.MiddleEntityAlias, inclusive,
																												componentData.ToArray());
			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(revisionTypePropName, "!=", QueryConstants.DelRevisionTypeParameter);
			// e.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(QueryConstants.ReferencedEntityAlias + "." + revisionTypePropName, false, "!=", QueryConstants.DelRevisionTypeParameter);
		}

		private void createValidAndRemovedDataRestrictions(IAuditStrategy auditStrategy,
																											 MiddleIdData referencedIdData, String versionsMiddleEntityName,
																											 QueryBuilder remQb, IEnumerable<MiddleComponentData> componentData)
		{
			var disjoint = remQb.RootParameters.AddSubParameters("or");
			var valid = disjoint.AddSubParameters("and"); // Restrictions to match all valid rows.
			var removed = disjoint.AddSubParameters("and"); // Restrictions to match all rows deleted at exactly given revision.
			var revisionPropertyPath = VerEntCfg.RevisionNumberPath;
			var revisionTypePropName = RevisionTypePath();
			// Excluding current revision, because we need to match data valid at the previous one.
			createValidDataRestrictions(auditStrategy, referencedIdData, versionsMiddleEntityName, remQb, valid, false, componentData);
			// ee.revision = :revision
			removed.AddWhereWithNamedParam(revisionPropertyPath, "=", QueryConstants.RevisionParameter);
			// e.revision = :revision
			removed.AddWhereWithNamedParam(QueryConstants.ReferencedEntityAlias + "." + revisionPropertyPath, false, "=", QueryConstants.RevisionParameter);
			// ee.revision_type = DEL
			removed.AddWhereWithNamedParam(revisionTypePropName, "=", QueryConstants.DelRevisionTypeParameter);
			// e.revision_type = DEL
			removed.AddWhereWithNamedParam(QueryConstants.ReferencedEntityAlias + "." + revisionTypePropName, false, "=", QueryConstants.DelRevisionTypeParameter);
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

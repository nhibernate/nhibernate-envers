using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from a relation middle-table and a two related versions entity.
	/// 
	/// Used when a map mapping of two entities is used.
	/// </summary>
	[Serializable]
	public sealed class ThreeEntityQueryGenerator : AbstractRelationQueryGenerator
	{
		private readonly string _queryString;
		private readonly string _queryRemovedString;

		public ThreeEntityQueryGenerator(AuditEntitiesConfiguration verEntCfg,
										IAuditStrategy auditStrategy,
										string versionsMiddleEntityName,
										MiddleIdData referencingIdData,
										MiddleIdData referencedIdData,
										MiddleIdData indexIdData,
										bool revisionTypeInId,
										IEnumerable<MiddleComponentData> componentDatas)
			: base(verEntCfg, referencingIdData, revisionTypeInId)
		{
			/*
			 * The query that we need to create:
			 *   SELECT new list(ee, e, f) FROM versionsReferencedEntity e, versionsIndexEntity f, middleEntity ee
			 *   WHERE
			 * (entities referenced by the middle table; id_ref_ed = id of the referenced entity)
			 *     ee.id_ref_ed = e.id_ref_ed AND
			 * (entities referenced by the middle table; id_ref_ind = id of the index entity)
			 *     ee.id_ref_ind = f.id_ref_ind AND
			 * (only entities referenced by the association; id_ref_ing = id of the referencing entity)
			 *     ee.id_ref_ing = :id_ref_ing AND
			 * (selecting e entities at revision :revision)
			 *	--> for DefaultAuditStrategy:
			 *		e.revision = (SELECT max(e2.revision) FROM versionsReferencedEntity e2
			 *			WHERE e2.revision <= :revision AND e2.id = e.id
			 *  --> for ValidityAuditStrategy:
			 *		e.revision <= :revision and (e.endRevision > :revision or e.endRevision is null)
			 * AND
			 * (selecting f entities at revision :revision)
			 *	--> for DefaultAuditStrategy:
			 *		f.revision = (SELECT max(f2.revision) FROM versionsIndexEntity f2
			 *			WHERE f2.revision <= :revision AND f2.id_ref_ed = f.id_ref_ed) 
			 *	--> for ValidityAuditStrategy:
			 *		f.revision <= :revision and (f.endRevision > :revision or f.endRevision is null)
			 * AND
			 * (the association at revision :revision)
			 *	--> for DefaultAuditStrategy:
			 *		ee.revision = (SELECT max(ee2.revision) FROM middleEntity ee2
			 *			WHERE ee2.revision <= :revision AND ee2.originalId.* = ee.originalId.*) 
			 *  --> for ValidityAuditStrategy:
			 *		ee.revision <= :revision and (ee.endRevision > :revision or ee.endRevision is null)
			 * AND
			 * (only non-deleted entities and associations)
			 *     ee.revision_type != DEL AND
			 *     e.revision_type != DEL AND
			 *     f.revision_type != DEL
			 */
			var commonPart = commonQueryPart(referencedIdData, indexIdData, versionsMiddleEntityName, verEntCfg.OriginalIdPropName);
			var validQuery = (QueryBuilder)commonPart.Clone();
			var removedQuery = (QueryBuilder)commonPart.Clone();

		  createValidDataRestrictions(auditStrategy, referencedIdData, versionsMiddleEntityName, validQuery,validQuery.RootParameters, true, componentDatas);
			createValidAndRemovedDataRestrictions(auditStrategy, referencedIdData, versionsMiddleEntityName, removedQuery, componentDatas);

			_queryString = QueryToString(validQuery);
			_queryRemovedString = QueryToString(removedQuery);
		}

		/// <summary>
		/// Compute common part for both queries.
		/// </summary>
		private QueryBuilder commonQueryPart(MiddleIdData referencedIdData, MiddleIdData indexIdData,
										 string versionsMiddleEntityName, string originalIdPropertyName)
		{
			var eeOriginalIdPropertyPath = QueryConstants.MiddleEntityAlias + "." + originalIdPropertyName;
			// SELECT new list(ee) FROM middleEntity ee
			var qb = new QueryBuilder(versionsMiddleEntityName, QueryConstants.MiddleEntityAlias);
			qb.AddFrom(referencedIdData.AuditEntityName, QueryConstants.ReferencedEntityAlias);
			qb.AddFrom(indexIdData.AuditEntityName, QueryConstants.IndexEntityAlias);
			qb.AddProjection("new list", QueryConstants.MiddleEntityAlias + ", "
												+ QueryConstants.ReferencedEntityAlias + ", "
												+ QueryConstants.IndexEntityAlias, false, false);
			// WHERE
			var rootParameters = qb.RootParameters;
			// ee.id_ref_ed = e.id_ref_ed
			referencedIdData.PrefixedMapper.AddIdsEqualToQuery(rootParameters, eeOriginalIdPropertyPath,
					referencedIdData.OriginalMapper, QueryConstants.ReferencedEntityAlias + "." + originalIdPropertyName);
			// ee.id_ref_ind = f.id_ref_ind
			indexIdData.PrefixedMapper.AddIdsEqualToQuery(rootParameters, eeOriginalIdPropertyPath,
					indexIdData.OriginalMapper, QueryConstants.IndexEntityAlias + "." + originalIdPropertyName);
			// ee.originalId.id_ref_ing = :id_ref_ing
			ReferencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);
			return qb;
		}

		/// <summary>
		/// Creates query restrictions used to retrieve only actual data.
		/// </summary>
		private void createValidDataRestrictions(IAuditStrategy auditStrategy,
																						 MiddleIdData referencedIdData, String versionsMiddleEntityName,
																						 QueryBuilder qb,
																						 Parameters rootParameters, bool inclusive,
																						 IEnumerable<MiddleComponentData> componentData)
		{
			var revisionPropertyPath = VerEntCfg.RevisionNumberPath;
			var originalIdPropertyName = VerEntCfg.OriginalIdPropName;
			var eeOriginalIdPropertyPath = QueryConstants.MiddleEntityAlias + "." + originalIdPropertyName;

			// (selecting e entities at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddEntityAtRevisionRestriction(qb, rootParameters, QueryConstants.ReferencedEntityAlias + "." + revisionPropertyPath,
						QueryConstants.ReferencedEntityAlias + "." + VerEntCfg.RevisionEndFieldName, false,
						referencedIdData, revisionPropertyPath, originalIdPropertyName, QueryConstants.ReferencedEntityAlias, QueryConstants.ReferencedEntityAliasDefAudStr);

			// (selecting f entities at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddEntityAtRevisionRestriction(qb, rootParameters, QueryConstants.IndexEntityAlias + "." + revisionPropertyPath,
						QueryConstants.IndexEntityAlias + "." + VerEntCfg.RevisionEndFieldName, false,
						referencedIdData, revisionPropertyPath, originalIdPropertyName, QueryConstants.IndexEntityAlias, QueryConstants.IndexEntityAliasDefAudStr);

			// (with ee association at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddAssociationAtRevisionRestriction(qb, rootParameters, revisionPropertyPath,
								 VerEntCfg.RevisionEndFieldName, true, ReferencingIdData, versionsMiddleEntityName,
								 eeOriginalIdPropertyPath, revisionPropertyPath, originalIdPropertyName, QueryConstants.MiddleEntityAlias, inclusive, componentData.ToArray());


			var revisionTypePropName = RevisionTypePath();
			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(revisionTypePropName, "!=", QueryConstants.DelRevisionTypeParameter);
			// e.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(QueryConstants.ReferencedEntityAlias + "." + revisionTypePropName, false, "!=", QueryConstants.DelRevisionTypeParameter);
			// f.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(QueryConstants.IndexEntityAlias + "." + revisionTypePropName, false, "!=", QueryConstants.DelRevisionTypeParameter);
		}

		/// <summary>
		/// Create query restrictions used to retrieve actual data and deletions that took place at exactly given revision.
		/// </summary>
		private void createValidAndRemovedDataRestrictions(IAuditStrategy auditStrategy,
														 MiddleIdData referencedIdData, string versionsMiddleEntityName,
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
			// f.revision = :revision
			removed.AddWhereWithNamedParam(QueryConstants.IndexEntityAlias + "." + revisionPropertyPath, false, "=", QueryConstants.RevisionParameter);
			// ee.revision_type = DEL
			removed.AddWhereWithNamedParam(revisionTypePropName, "=", QueryConstants.DelRevisionTypeParameter);
			// e.revision_type = DEL
			removed.AddWhereWithNamedParam(QueryConstants.ReferencedEntityAlias + "." + revisionTypePropName, false, "=", QueryConstants.DelRevisionTypeParameter);
			// f.revision_type = DEL
			removed.AddWhereWithNamedParam(QueryConstants.IndexEntityAlias + "." + revisionTypePropName, false, "=", QueryConstants.DelRevisionTypeParameter);
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

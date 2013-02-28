using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from a relation middle-table and a related versions entity.
	/// </summary>
	[Serializable]
	public sealed class TwoEntityQueryGenerator : AbstractRelationQueryGenerator
	{
		private readonly string _queryString;

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
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;

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
			referencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);

			// (selecting e entities at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddEntityAtRevisionRestriction(qb, QueryConstants.ReferencedEntityAlias + "." + revisionPropertyPath,
			      QueryConstants.ReferencedEntityAlias + "." + verEntCfg.RevisionEndFieldName, false,
					referencedIdData, revisionPropertyPath, originalIdPropertyName, QueryConstants.ReferencedEntityAlias, QueryConstants.ReferencedEntityAliasDefAudStr);

			// (with ee association at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddAssociationAtRevisionRestriction(qb, revisionPropertyPath,
															  verEntCfg.RevisionEndFieldName, true, referencingIdData,
															  versionsMiddleEntityName,
															  eeOriginalIdPropertyPath, revisionPropertyPath,
															  originalIdPropertyName, QueryConstants.MiddleEntityAlias, componentDatas.ToArray());

			var revisionTypePropName = RevisionTypePath();
			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(revisionTypePropName, "!=", QueryConstants.DelRevisionTypeParameter);
			// e.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(QueryConstants.ReferencedEntityAlias + "." + revisionTypePropName, false, "!=", QueryConstants.DelRevisionTypeParameter);

			var sb = new StringBuilder();
			qb.Build(sb, null);
			_queryString = sb.ToString();
		}

		protected override string QueryString()
		{
			return _queryString;
		}
	}
}

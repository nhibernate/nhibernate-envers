using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from a relation middle-table only.
	/// </summary>
	[Serializable]
	public sealed class OneEntityQueryGenerator : AbstractRelationQueryGenerator 
	{
		private readonly string _queryString;

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
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;

			// SELECT new list(ee) FROM middleEntity ee
			var qb = new QueryBuilder(versionsMiddleEntityName, QueryConstants.MiddleEntityAlias);
			qb.AddProjection("new list", QueryConstants.MiddleEntityAlias, false, false);
			// WHERE
			var rootParameters = qb.RootParameters;
			// ee.originalId.id_ref_ing = :id_ref_ing
			referencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);


			var eeOriginalIdPropertyPath = QueryConstants.MiddleEntityAlias + "." + originalIdPropertyName;

			// (with ee association at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddAssociationAtRevisionRestriction(qb, revisionPropertyPath,
				verEntCfg.RevisionEndFieldName, true, referencingIdData, versionsMiddleEntityName, 
				eeOriginalIdPropertyPath, revisionPropertyPath, originalIdPropertyName, QueryConstants.MiddleEntityAlias, componentDatas.ToArray());

			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(RevisionTypePath(), "!=", QueryConstants.DelRevisionTypeParameter);

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

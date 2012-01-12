using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from a relation middle-table only.
	/// </summary>
	public sealed class OneEntityQueryGenerator : IRelationQueryGenerator 
	{
		private readonly string _queryString;
		private readonly MiddleIdData _referencingIdData;

		public OneEntityQueryGenerator(AuditEntitiesConfiguration verEntCfg,
										IAuditStrategy auditStrategy,
										string versionsMiddleEntityName,
										MiddleIdData referencingIdData,
										IEnumerable<MiddleComponentData> componentDatas) 
		{
			_referencingIdData = referencingIdData;

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
			var qb = new QueryBuilder(versionsMiddleEntityName, "ee");
			qb.AddProjection("new list", "ee", false, false);
			// WHERE
			var rootParameters = qb.RootParameters;
			// ee.originalId.id_ref_ing = :id_ref_ing
			referencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);


			var eeOriginalIdPropertyPath = "ee." + originalIdPropertyName;

			// (with ee association at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddAssociationAtRevisionRestriction(qb, revisionPropertyPath,
				verEntCfg.RevisionEndFieldName, true, referencingIdData, versionsMiddleEntityName, 
				eeOriginalIdPropertyPath, revisionPropertyPath, originalIdPropertyName, componentDatas.ToArray());

			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(verEntCfg.RevisionTypePropName, "!=", "delrevisiontype");

			var sb = new StringBuilder();
			qb.Build(sb, null);
			_queryString = sb.ToString();
		}

		public IQuery GetQuery(IAuditReaderImplementor versionsReader, object primaryKey, long revision) 
		{
			var query = versionsReader.Session.CreateQuery(_queryString);
			query.SetParameter("revision", revision);
			query.SetParameter("delrevisiontype", RevisionType.Deleted);
			foreach (var paramData in _referencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey))
			{
				paramData.SetParameterValue(query);
			}

			return query;
		}
	}
}

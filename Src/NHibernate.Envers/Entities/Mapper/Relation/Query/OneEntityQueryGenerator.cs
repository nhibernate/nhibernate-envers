using System.Collections.Generic;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

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
			 *     ee.revision = (SELECT max(ee2.revision) FROM middleEntity ee2
			 *       WHERE ee2.revision <= :revision AND ee2.originalId.* = ee.originalId.*) AND
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
			// SELECT max(ee2.revision) FROM middleEntity ee2
			var maxRevQb = qb.NewSubQueryBuilder(versionsMiddleEntityName, "ee2");
			maxRevQb.AddProjection("max", revisionPropertyPath, false);
			// WHERE
			var maxRevQbParameters = maxRevQb.RootParameters;
			// ee2.revision <= :revision
			maxRevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, "<=", "revision");
			// ee2.originalId.* = ee.originalId.*        
			var eeOriginalIdPropertyPath = "ee." + originalIdPropertyName;
			var ee2OriginalIdPropertyPath = "ee2." + originalIdPropertyName;
			referencingIdData.PrefixedMapper.AddIdsEqualToQuery(maxRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);
			foreach (var componentData in componentDatas) 
			{
				componentData.ComponentMapper.AddMiddleEqualToQuery(maxRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);
			}
			// ee.revision = (SELECT max(...) ...)
			rootParameters.AddWhere(revisionPropertyPath, "=", maxRevQb);       
			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(verEntCfg.RevisionTypePropName, "!=", "delrevisiontype");

			var sb = new StringBuilder();
			qb.Build(sb, EmptyDictionary<string, object>.Instance);
			_queryString = sb.ToString();
		}

		public IQuery GetQuery(IAuditReaderImplementor versionsReader, object primaryKey, long revision) 
		{
			var query = versionsReader.Session.CreateQuery(_queryString);
			query.SetParameter("revision", revision);
			query.SetParameter("delrevisiontype", RevisionType.DEL);
			foreach (var paramData in _referencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey))
			{
				paramData.SetParameterValue(query);
			}

			return query;
		}
	}
}

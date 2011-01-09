using System;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Transform;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	public sealed class OneAuditEntityQueryGenerator: IRelationQueryGenerator 
	{
		private readonly string queryString;
		private readonly MiddleIdData referencingIdData;

		public OneAuditEntityQueryGenerator(GlobalConfiguration globalCfg, 
											AuditEntitiesConfiguration verEntCfg,
											MiddleIdData referencingIdData, 
											string referencedEntityName,
											IIdMapper referencedIdMapper) 
		{
			this.referencingIdData = referencingIdData;

			/*
			 * The query that we need to create:
			 *   SELECT new list(e) FROM versionsReferencedEntity e
			 *   WHERE
			 * (only entities referenced by the association; id_ref_ing = id of the referencing entity)
			 *     e.id_ref_ing = :id_ref_ing AND
			 * (selecting e entities at revision :revision)
			 *     e.revision = (SELECT max(e2.revision) FROM versionsReferencedEntity e2
			 *       WHERE e2.revision <= :revision AND e2.id = e.id) AND
			 * (only non-deleted entities)
			 *     e.revision_type != DEL
			 */
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;
			var versionsReferencedEntityName = verEntCfg.GetAuditEntityName(referencedEntityName);

			// SELECT new list(e) FROM versionsEntity e
			var qb = new QueryBuilder(versionsReferencedEntityName, "e");
			//qb.AddProjection("new list", "e", false, false);
			// WHERE
			var rootParameters = qb.RootParameters;
			// e.id_ref_ed = :id_ref_ed
			referencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, null, true);

			// SELECT max(e.revision) FROM versionsReferencedEntity e2
			var maxERevQb = qb.NewSubQueryBuilder(versionsReferencedEntityName, "e2");
			maxERevQb.AddProjection("max", revisionPropertyPath, false);
			// WHERE
			var maxERevQbParameters = maxERevQb.RootParameters;
			// e2.revision <= :revision
			maxERevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, "<=", "revision");
			// e2.id = e.id
			referencedIdMapper.AddIdsEqualToQuery(maxERevQbParameters,
					"e." + originalIdPropertyName, "e2." + originalIdPropertyName);

			// e.revision = (SELECT max(...) ...)
			rootParameters.AddWhere(revisionPropertyPath, false, globalCfg.getCorrelatedSubqueryOperator(), maxERevQb);

			// e.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(verEntCfg.RevisionTypePropName, false, "!=", "delrevisiontype");

			var sb = new StringBuilder();
			qb.Build(sb, EmptyDictionary<String, object>.Instance);
			queryString = sb.ToString();
		}

		public IQuery GetQuery(IAuditReaderImplementor versionsReader, object primaryKey, long revision) 
		{
			var query = versionsReader.Session.CreateQuery(queryString);
			query.SetParameter("revision", revision);
			query.SetParameter("delrevisiontype", RevisionType.DEL.Representation);
			foreach (var paramData in referencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey)) 
			{
				paramData.SetParameterValue(query);
			}
			query.SetResultTransformer(Transformers.ToList);
			return query;
		}
	}
}
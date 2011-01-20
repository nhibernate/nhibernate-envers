using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/**
	 * Selects data from a relation middle-table and a related non-audited entity.
	 * @author Adam Warski (adam at warski dot org)
	 */
	public sealed class TwoEntityOneAuditedQueryGenerator : IRelationQueryGenerator 
	{
		private readonly String queryString;
		private readonly MiddleIdData referencingIdData;

		public TwoEntityOneAuditedQueryGenerator(
									   AuditEntitiesConfiguration verEntCfg,
									   String versionsMiddleEntityName,
									   MiddleIdData referencingIdData,
									   MiddleIdData referencedIdData,
									   IEnumerable<MiddleComponentData> componentDatas) 
		{
			this.referencingIdData = referencingIdData;

			/*
			 * The query that we need to create:
			 *   SELECT new list(ee, e) FROM referencedEntity e, middleEntity ee
			 *   WHERE
			 * (entities referenced by the middle table; id_ref_ed = id of the referenced entity)
			 *     ee.id_ref_ed = e.id_ref_ed AND
			 * (only entities referenced by the association; id_ref_ing = id of the referencing entity)
			 *     ee.id_ref_ing = :id_ref_ing AND
			 * (the association at revision :revision)
			 *     ee.revision = (SELECT max(ee2.revision) FROM middleEntity ee2
			 *       WHERE ee2.revision <= :revision AND ee2.originalId.* = ee.originalId.*) AND
			 * (only non-deleted entities and associations)
			 *     ee.revision_type != DEL
			 */
			String revisionPropertyPath = verEntCfg.RevisionNumberPath;
			String originalIdPropertyName = verEntCfg.OriginalIdPropName;

			String eeOriginalIdPropertyPath = "ee." + originalIdPropertyName;

			// SELECT new list(ee) FROM middleEntity ee
			QueryBuilder qb = new QueryBuilder(versionsMiddleEntityName, "ee");
			qb.AddFrom(referencedIdData.EntityName, "e");
			qb.AddProjection("new list", "ee, e", false, false);
			// WHERE
			Parameters rootParameters = qb.RootParameters;
			// ee.id_ref_ed = e.id_ref_ed
			referencedIdData.PrefixedMapper.AddIdsEqualToQuery(rootParameters, eeOriginalIdPropertyPath,
					referencedIdData.OriginalMapper, "e");
			// ee.originalId.id_ref_ing = :id_ref_ing
			referencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);

			// ee.revision = (SELECT max(...) ...)
			QueryGeneratorTools.AddAssociationAtRevision(qb, rootParameters, referencingIdData, versionsMiddleEntityName,
					eeOriginalIdPropertyPath, revisionPropertyPath, originalIdPropertyName, componentDatas);

			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(verEntCfg.RevisionTypePropName, "!=", "delrevisiontype");

			StringBuilder sb = new StringBuilder();
			qb.Build(sb, EmptyDictionary<String, Object>.Instance);
			queryString = sb.ToString();
		}


		public IQuery GetQuery(IAuditReaderImplementor versionsReader, Object primaryKey, long revision)
		{
			IQuery query = versionsReader.Session.CreateQuery(queryString);
			query.SetParameter("revision", revision);
			query.SetParameter("delrevisiontype", RevisionType.DEL);
			foreach (QueryParameterData paramData in referencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey))
			{
				paramData.SetParameterValue(query);
			}

			return query;
		}
	}
}

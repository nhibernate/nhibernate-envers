using System.Collections.Generic;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/**
	 * Selects data from a relation middle-table and a two related versions entity.
	 * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
	 */
	public sealed class ThreeEntityQueryGenerator : IRelationQueryGenerator 
	{
		private readonly string queryString;
		private readonly MiddleIdData referencingIdData;

		public ThreeEntityQueryGenerator(GlobalConfiguration globalCfg,
										AuditEntitiesConfiguration verEntCfg,
										string versionsMiddleEntityName,
										MiddleIdData referencingIdData,
										MiddleIdData referencedIdData,
										MiddleIdData indexIdData,
										IEnumerable<MiddleComponentData> componentDatas) 
		{
			this.referencingIdData = referencingIdData;

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
			 *     e.revision = (SELECT max(e2.revision) FROM versionsReferencedEntity e2
			 *       WHERE e2.revision <= :revision AND e2.id_ref_ed = e.id_ref_ed) AND
			 * (selecting f entities at revision :revision)
			 *     f.revision = (SELECT max(f2.revision) FROM versionsIndexEntity f2
			 *       WHERE f2.revision <= :revision AND f2.id_ref_ed = f.id_ref_ed) AND
			 * (the association at revision :revision)
			 *     ee.revision = (SELECT max(ee2.revision) FROM middleEntity ee2
			 *       WHERE ee2.revision <= :revision AND ee2.originalId.* = ee.originalId.*) AND
			 * (only non-deleted entities and associations)
			 *     ee.revision_type != DEL AND
			 *     e.revision_type != DEL AND
			 *     f.revision_type != DEL
			 */
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;

			var eeOriginalIdPropertyPath = "ee." + originalIdPropertyName;

			// SELECT new list(ee) FROM middleEntity ee
			var qb = new QueryBuilder(versionsMiddleEntityName, "ee");
			qb.AddFrom(referencedIdData.AuditEntityName, "e");
			qb.AddFrom(indexIdData.AuditEntityName, "f");
			qb.AddProjection("new list", "ee, e, f", false, false);
			// WHERE
			var rootParameters = qb.RootParameters;
			// ee.id_ref_ed = e.id_ref_ed
			referencedIdData.PrefixedMapper.AddIdsEqualToQuery(rootParameters, eeOriginalIdPropertyPath,
					referencedIdData.OriginalMapper, "e." + originalIdPropertyName);
			// ee.id_ref_ind = f.id_ref_ind
			indexIdData.PrefixedMapper.AddIdsEqualToQuery(rootParameters, eeOriginalIdPropertyPath,
					indexIdData.OriginalMapper, "f." + originalIdPropertyName);
			// ee.originalId.id_ref_ing = :id_ref_ing
			referencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);

			// e.revision = (SELECT max(...) ...)
			QueryGeneratorTools.AddEntityAtRevision(globalCfg, qb, rootParameters, referencedIdData, revisionPropertyPath,
					originalIdPropertyName, "e", "e2");

			// f.revision = (SELECT max(...) ...)
			QueryGeneratorTools.AddEntityAtRevision(globalCfg, qb, rootParameters, indexIdData, revisionPropertyPath,
					originalIdPropertyName, "f", "f2");

			// ee.revision = (SELECT max(...) ...)
			QueryGeneratorTools.AddAssociationAtRevision(qb, rootParameters, referencingIdData, versionsMiddleEntityName,
					eeOriginalIdPropertyPath, revisionPropertyPath, originalIdPropertyName, componentDatas);

			// ee.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(verEntCfg.RevisionTypePropName, "!=", "delrevisiontype");
			// e.revision_type != DEL
			rootParameters.AddWhereWithNamedParam("e." + verEntCfg.RevisionTypePropName, false, "!=", "delrevisiontype");
			// f.revision_type != DEL
			rootParameters.AddWhereWithNamedParam("f." + verEntCfg.RevisionTypePropName, false, "!=", "delrevisiontype");

			var sb = new StringBuilder();
			qb.Build(sb, EmptyDictionary<string, object>.Instance);
			queryString = sb.ToString();
		}

		public IQuery GetQuery(IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			var query = versionsReader.Session.CreateQuery(queryString);
			query.SetParameter("revision", revision);
			query.SetParameter("delrevisiontype", RevisionType.DEL);
			foreach (var paramData in referencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey))
			{
				paramData.SetParameterValue(query);
			}

			return query;
		}
	}
}

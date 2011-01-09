using System;
using System.Collections.Generic;
using System.Linq;
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
     * Selects data from a relation middle-table only.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public sealed class OneEntityQueryGenerator : IRelationQueryGenerator {
        private readonly String _queryString;
        private readonly MiddleIdData _referencingIdData;

        public OneEntityQueryGenerator(AuditEntitiesConfiguration verEntCfg,
                                       String versionsMiddleEntityName,
                                       MiddleIdData referencingIdData,
                                       IEnumerable<MiddleComponentData> componentDatas) {
            this._referencingIdData = referencingIdData;

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
            String revisionPropertyPath = verEntCfg.RevisionNumberPath;
            String originalIdPropertyName = verEntCfg.OriginalIdPropName;

            // SELECT new list(ee) FROM middleEntity ee
            QueryBuilder qb = new QueryBuilder(versionsMiddleEntityName, "ee");
            qb.AddProjection("new list", "ee", false, false);
            // WHERE
            Parameters rootParameters = qb.RootParameters;
            // ee.originalId.id_ref_ing = :id_ref_ing
            referencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, originalIdPropertyName, true);
            // SELECT max(ee2.revision) FROM middleEntity ee2
            QueryBuilder maxRevQb = qb.NewSubQueryBuilder(versionsMiddleEntityName, "ee2");
            maxRevQb.AddProjection("max", revisionPropertyPath, false);
            // WHERE
            Parameters maxRevQbParameters = maxRevQb.RootParameters;
            // ee2.revision <= :revision
            maxRevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, "<=", "revision");
            // ee2.originalId.* = ee.originalId.*        
            String eeOriginalIdPropertyPath = "ee." + originalIdPropertyName;
            String ee2OriginalIdPropertyPath = "ee2." + originalIdPropertyName;
            referencingIdData.PrefixedMapper.AddIdsEqualToQuery(maxRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);
            foreach (MiddleComponentData componentData in componentDatas) {
                componentData.ComponentMapper.AddMiddleEqualToQuery(maxRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);
            }
            // ee.revision = (SELECT max(...) ...)
            rootParameters.AddWhere(revisionPropertyPath, "=", maxRevQb);       
            // ee.revision_type != DEL
            rootParameters.AddWhereWithNamedParam(verEntCfg.RevisionTypePropName, "!=", "delrevisiontype");

            StringBuilder sb = new StringBuilder();
            qb.Build(sb, EmptyDictionary<String, object>.Instance);
            _queryString = sb.ToString();
        }

        public IQuery GetQuery(IAuditReaderImplementor versionsReader, Object primaryKey, long revision) {
            IQuery query = versionsReader.Session.CreateQuery(_queryString);
            query.SetParameter("revision", revision);
            query.SetParameter("delrevisiontype", RevisionType.DEL.Representation);
            foreach (QueryParameterData paramData in _referencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey)) {
                paramData.SetParameterValue(query);
            }

            return query;
        }
    }
}

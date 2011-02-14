﻿using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
    public static class QueryGeneratorTools 
	{
        public static void AddEntityAtRevision(GlobalConfiguration globalCfg, QueryBuilder qb, Parameters rootParameters,
											   MiddleIdData idData, string revisionPropertyPath, string originalIdPropertyName,
											   string alias1, string alias2)
		{
            // SELECT max(e.revision) FROM versionsReferencedEntity e2
            var maxERevQb = qb.NewSubQueryBuilder(idData.AuditEntityName, alias2);
            maxERevQb.AddProjection("max", revisionPropertyPath, false);
            // WHERE
            var maxERevQbParameters = maxERevQb.RootParameters;
            // e2.revision <= :revision
            maxERevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, "<=", "revision");
            // e2.id_ref_ed = e.id_ref_ed
            idData.OriginalMapper.AddIdsEqualToQuery(maxERevQbParameters,
                    alias1 + "." + originalIdPropertyName, alias2 +"." + originalIdPropertyName);

            // e.revision = (SELECT max(...) ...)
            rootParameters.AddWhere("e." + revisionPropertyPath, false, globalCfg.CorrelatedSubqueryOperator, maxERevQb);
        }

        public static void AddAssociationAtRevision(QueryBuilder qb, Parameters rootParameters,
                                                    MiddleIdData referencingIdData, string versionsMiddleEntityName,
													string eeOriginalIdPropertyPath, string revisionPropertyPath,
													string originalIdPropertyName, IEnumerable<MiddleComponentData> componentDatas)
		{
            // SELECT max(ee2.revision) FROM middleEntity ee2
            var maxEeRevQb = qb.NewSubQueryBuilder(versionsMiddleEntityName, "ee2");
            maxEeRevQb.AddProjection("max", revisionPropertyPath, false);
            // WHERE
            var maxEeRevQbParameters = maxEeRevQb.RootParameters;
            // ee2.revision <= :revision
            maxEeRevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, "<=", "revision");
            // ee2.originalId.* = ee.originalId.*
			var ee2OriginalIdPropertyPath = "ee2." + originalIdPropertyName;
            referencingIdData.PrefixedMapper.AddIdsEqualToQuery(maxEeRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);
            foreach (var componentData in componentDatas) 
			{
                componentData.ComponentMapper.AddMiddleEqualToQuery(maxEeRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);
            }

            // ee.revision = (SELECT max(...) ...)
            rootParameters.AddWhere(revisionPropertyPath, "=", maxEeRevQb);
        }
    }
}

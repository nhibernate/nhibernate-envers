using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Impl
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class EntitiesAtRevisionQuery : AbstractAuditQuery {
        private readonly long revision;

        public EntitiesAtRevisionQuery(AuditConfiguration verCfg,
                                       IAuditReaderImplementor versionsReader, System.Type cls,
                                       long revision) 
        : base(verCfg, versionsReader, cls)
        {
            
            this.revision = revision;
        }

        public override IList List()
        {
            /*
            The query that should be executed in the versions table:
            SELECT e FROM ent_ver e WHERE
              (all specified conditions, transformed, on the "e" entity) AND
              e.revision_type != DEL AND
              e.revision = (SELECT max(e2.revision) FROM ent_ver e2 WHERE
                e2.revision <= :revision AND e2.originalId.id = e.originalId.id)
             */

            QueryBuilder maxRevQb = qb.NewSubQueryBuilder(versionsEntityName, "e2");

            AuditEntitiesConfiguration verEntCfg = verCfg.AuditEntCfg;

            String revisionPropertyPath = verEntCfg.RevisionNumberPath;
            String originalIdPropertyName = verEntCfg.OriginalIdPropName;

            // SELECT max(e2.revision)
            maxRevQb.AddProjection("max", revisionPropertyPath, false);
            // e2.revision <= :revision
            maxRevQb.RootParameters.AddWhereWithParam(revisionPropertyPath, "<=", revision);
            // e2.id = e.id
            verCfg.EntCfg[entityName].GetIdMapper().AddIdsEqualToQuery(maxRevQb.RootParameters,
                                                                       "e." + originalIdPropertyName,
                                                                       "e2." + originalIdPropertyName);

            // e.revision_type != DEL AND
            qb.RootParameters.AddWhereWithParam(verEntCfg.RevisionTypePropName, "<>", RevisionType.DEL);
            // e.revision = (SELECT max(...) ...)
            qb.RootParameters.AddWhere(revisionPropertyPath, verCfg.GlobalCfg.getCorrelatedSubqueryOperator(), maxRevQb);
            // all specified conditions
            foreach (IAuditCriterion criterion in criterions)
            {
                criterion.AddToQuery(verCfg, entityName, qb, qb.RootParameters);
            }
            IList res = BuildAndExecuteQuery();
            IList queryResult = res;

            if (hasProjection)
            {
                return queryResult;
            }
            var result = new ArrayList();
            var queryResultTyped = new List<IDictionary<string, object>>();
            foreach (IDictionary hash in queryResult)
            {
                queryResultTyped.Add(DictionaryWrapper<string, object>.Wrap(hash));
            }
            entityInstantiator.AddInstancesFromVersionsEntities(entityName, result,
                                                                queryResultTyped, revision);
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * A criterion that expresses that the id of an entity is equal or not equal to some specified value.
     * @author Adam Warski (adam at warski dot org)
     */
    public class IdentifierEqAuditExpression : IAuditCriterion {
        private readonly Object id;
        private readonly bool equals;

        public IdentifierEqAuditExpression(Object id, bool equals) {
            this.id = id;
            this.equals = equals;
        }

        public void AddToQuery(AuditConfiguration verCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            verCfg.EntCfg[entityName].GetIdMapper()
                    .AddIdEqualsToQuery(parameters, id, verCfg.AuditEntCfg.OriginalIdPropName, equals);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
    {
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class NotAuditExpression : IAuditCriterion {
        private IAuditCriterion criterion;

        public NotAuditExpression(IAuditCriterion criterion) {
            this.criterion = criterion;
        }

        public void AddToQuery(AuditConfiguration verCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            criterion.AddToQuery(verCfg, entityName, qb, parameters.AddNegatedParameters());
        }
    }
}

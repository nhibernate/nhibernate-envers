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
    public class LogicalAuditExpression : IAuditCriterion {
        private IAuditCriterion lhs;
        private IAuditCriterion rhs;
        private String op;

        public LogicalAuditExpression(IAuditCriterion lhs, IAuditCriterion rhs, String op) {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public void AddToQuery(AuditConfiguration verCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            Parameters opParameters = parameters.AddSubParameters(op);

            lhs.AddToQuery(verCfg, entityName, qb, opParameters.AddSubParameters("and"));
            rhs.AddToQuery(verCfg, entityName, qb, opParameters.AddSubParameters("and"));
        }
    }
}

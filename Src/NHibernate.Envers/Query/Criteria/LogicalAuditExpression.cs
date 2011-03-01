using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
    public class LogicalAuditExpression : IAuditCriterion 
	{
        private readonly IAuditCriterion lhs;
        private readonly IAuditCriterion rhs;
        private readonly string op;

        public LogicalAuditExpression(IAuditCriterion lhs, IAuditCriterion rhs, string op) 
		{
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public void AddToQuery(AuditConfiguration verCfg, String entityName, QueryBuilder qb, Parameters parameters) 
		{
            var opParameters = parameters.AddSubParameters(op);

            lhs.AddToQuery(verCfg, entityName, qb, opParameters.AddSubParameters("and"));
            rhs.AddToQuery(verCfg, entityName, qb, opParameters.AddSubParameters("and"));
        }
    }
}

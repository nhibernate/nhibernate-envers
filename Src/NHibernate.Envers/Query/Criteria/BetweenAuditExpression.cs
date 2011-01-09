using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class BetweenAuditExpression : IAuditCriterion {
        private IPropertyNameGetter propertyNameGetter;
        private Object lo;
        private Object hi;

        public BetweenAuditExpression(IPropertyNameGetter propertyNameGetter, Object lo, Object hi) {
            this.propertyNameGetter = propertyNameGetter;
            this.lo = lo;
            this.hi = hi;
        }

        public void AddToQuery(AuditConfiguration auditCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            String propertyName = propertyNameGetter.Get(auditCfg);
            CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);
            parameters.AddWhereWithParam(propertyName, ">=", lo);
            parameters.AddWhereWithParam(propertyName, "<=", hi);
        }
    }
}

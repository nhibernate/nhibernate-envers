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
    public class InAuditExpression : IAuditCriterion {
        private IPropertyNameGetter propertyNameGetter;
        private Object[] values;

        public InAuditExpression(IPropertyNameGetter propertyNameGetter, Object[] values) {
            this.propertyNameGetter = propertyNameGetter;
            this.values = values;
        }

        public void AddToQuery(AuditConfiguration auditCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            String propertyName = propertyNameGetter.Get(auditCfg);
            CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);
            parameters.AddWhereWithParams(propertyName, "in (", values, ")");
        }
    }
}

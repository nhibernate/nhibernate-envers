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
    public class PropertyAuditExpression : IAuditCriterion {
        private IPropertyNameGetter propertyNameGetter;
        private String otherPropertyName;
        private String op;

        public PropertyAuditExpression(IPropertyNameGetter propertyNameGetter, String otherPropertyName, String op) {
            this.propertyNameGetter = propertyNameGetter;
            this.otherPropertyName = otherPropertyName;
            this.op = op;
        }

        public void AddToQuery(AuditConfiguration auditCfg, String entityName, QueryBuilder qb, Parameters parameters) {   
            String propertyName = propertyNameGetter.Get(auditCfg);
            CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);
            CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, otherPropertyName);
            parameters.AddWhere(propertyName, op, otherPropertyName);
        }
    }
}

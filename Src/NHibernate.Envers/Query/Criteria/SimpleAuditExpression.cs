using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class SimpleAuditExpression : IAuditCriterion {
        private readonly IPropertyNameGetter propertyNameGetter;
        private readonly Object value;
        private readonly String op;

        public SimpleAuditExpression(IPropertyNameGetter propertyNameGetter, Object value, String op) {
            this.propertyNameGetter = propertyNameGetter;
            this.value = value;
            this.op = op;
        }

        public void AddToQuery(AuditConfiguration auditCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            String propertyName = propertyNameGetter.Get(auditCfg);

            RelationDescription relatedEntity = CriteriaTools.GetRelatedEntity(auditCfg, entityName, propertyName);

            if (relatedEntity == null) {
                parameters.AddWhereWithParam(propertyName, op, value);
            } else {
                if (!"=".Equals(op) && !"<>".Equals(op)) {
                    throw new AuditException("This type of operation: " + op + " (" + entityName + "." + propertyName +
                            ") isn't supported and can't be used in queries.");
                }

                Object id = relatedEntity.IdMapper.MapToIdFromEntity(value);

                relatedEntity.IdMapper.AddIdEqualsToQuery(parameters, id, propertyName, "=".Equals(op));
            }
        }
    }
}

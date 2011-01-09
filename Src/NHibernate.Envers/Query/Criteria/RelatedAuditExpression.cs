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
    public class RelatedAuditExpression : IAuditCriterion {
        private readonly IPropertyNameGetter propertyNameGetter;
        private readonly Object id;
        private readonly bool equals;

        public RelatedAuditExpression(IPropertyNameGetter propertyNameGetter, Object id, bool equals) {
            this.propertyNameGetter = propertyNameGetter;
            this.id = id;
            this.equals = equals;
        }

        public void AddToQuery(AuditConfiguration auditCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            String propertyName = propertyNameGetter.Get(auditCfg);
            
            RelationDescription relatedEntity = CriteriaTools.GetRelatedEntity(auditCfg, entityName, propertyName);

            if (relatedEntity == null) {
                throw new AuditException("This criterion can only be used on a property that is " +
                        "a relation to another property.");
            } else {
                relatedEntity.IdMapper.AddIdEqualsToQuery(parameters, id, propertyName, equals);
            }
        }
    }
}

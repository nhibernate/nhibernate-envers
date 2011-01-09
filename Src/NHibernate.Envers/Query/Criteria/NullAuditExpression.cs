using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class NullAuditExpression : IAuditCriterion {
        private IPropertyNameGetter propertyNameGetter;

        public NullAuditExpression(IPropertyNameGetter propertyNameGetter) {
            this.propertyNameGetter = propertyNameGetter;
        }

        public void AddToQuery(AuditConfiguration auditCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            String propertyName = propertyNameGetter.Get(auditCfg);
            RelationDescription relatedEntity = CriteriaTools.GetRelatedEntity(auditCfg, entityName, propertyName);

            if (relatedEntity == null) {
                parameters.AddWhereWithParam(propertyName, "=", null);
            } else {
                relatedEntity.IdMapper.AddIdEqualsToQuery(parameters, null, propertyName, true);
            }
        }
    }
}

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
    public class AggregatedAuditExpression : IAuditCriterion, IExtendableCriterion {
        private IPropertyNameGetter propertyNameGetter;
        private AggregatedMode mode;
        private IList<IAuditCriterion> criterions;

        public AggregatedAuditExpression(IPropertyNameGetter propertyNameGetter, AggregatedMode mode) {
            this.propertyNameGetter = propertyNameGetter;
            this.mode = mode;
            criterions = new List<IAuditCriterion>();
        }

        public enum AggregatedMode {
            MAX,
            MIN
        }

        public IExtendableCriterion Add(IAuditCriterion criterion) {
            criterions.Add(criterion);
            return this;
        }

        public void AddToQuery(AuditConfiguration auditCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            String propertyName = propertyNameGetter.Get(auditCfg);

            CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);

            // This will be the aggregated query, containing all the specified conditions
            QueryBuilder subQb = qb.NewSubQueryBuilder();

            // Adding all specified conditions both to the main query, as well as to the
            // aggregated one.
            foreach (IAuditCriterion versionsCriteria in criterions) {
                versionsCriteria.AddToQuery(auditCfg, entityName, qb, parameters);
                versionsCriteria.AddToQuery(auditCfg, entityName, subQb, subQb.RootParameters);
            }

            // Setting the desired projection of the aggregated query
            switch (mode) {
                case AggregatedMode.MIN:
                    subQb.AddProjection("min", propertyName, false);
                    break;
                case AggregatedMode.MAX:
                    subQb.AddProjection("max", propertyName, false);
                    break;
            }

            // Adding the constrain on the result of the aggregated criteria
            parameters.AddWhere(propertyName, "=", subQb);
        }
    }
}

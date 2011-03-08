using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
    public class AggregatedAuditExpression : IExtendableCriterion 
	{
        private readonly IPropertyNameGetter propertyNameGetter;
        private readonly AggregatedMode mode;
        private readonly IList<IAuditCriterion> criterions;

        public AggregatedAuditExpression(IPropertyNameGetter propertyNameGetter, AggregatedMode mode) 
		{
            this.propertyNameGetter = propertyNameGetter;
            this.mode = mode;
            criterions = new List<IAuditCriterion>();
        }

        public enum AggregatedMode 
		{
            Max,
            Min
        }

        public IExtendableCriterion Add(IAuditCriterion criterion) 
		{
            criterions.Add(criterion);
            return this;
        }

        public void AddToQuery(AuditConfiguration auditCfg, string entityName, QueryBuilder qb, Parameters parameters) 
		{
            var propertyName = propertyNameGetter.Get(auditCfg);

            CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);

            // This will be the aggregated query, containing all the specified conditions
            var subQb = qb.NewSubQueryBuilder();

            // Adding all specified conditions both to the main query, as well as to the
            // aggregated one.
            foreach (var versionsCriteria in criterions) 
			{
                versionsCriteria.AddToQuery(auditCfg, entityName, qb, parameters);
                versionsCriteria.AddToQuery(auditCfg, entityName, subQb, subQb.RootParameters);
            }

            // Setting the desired projection of the aggregated query
            switch (mode) 
			{
                case AggregatedMode.Min:
                    subQb.AddProjection("min", propertyName, false);
                    break;
                case AggregatedMode.Max:
                    subQb.AddProjection("max", propertyName, false);
                    break;
            }

            // Adding the constrain on the result of the aggregated criteria
            parameters.AddWhere(propertyName, "=", subQb);
        }
    }
}

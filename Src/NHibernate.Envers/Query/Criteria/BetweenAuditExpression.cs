using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class BetweenAuditExpression : IAuditCriterion
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly object lo;
		private readonly object hi;

		public BetweenAuditExpression(IPropertyNameGetter propertyNameGetter, object lo, object hi)
		{
			this.propertyNameGetter = propertyNameGetter;
			this.lo = lo;
			this.hi = hi;
		}

		public void AddToQuery(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters)
		{
			var propertyName = CriteriaTools.DeterminePropertyName(auditCfg, versionsReader, entityName, propertyNameGetter);
			CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);
			var subParams = parameters.AddSubParameters(Parameters.AND);
			subParams.AddWhereWithParam(propertyName, ">=", lo);
			subParams.AddWhereWithParam(propertyName, "<=", hi);
		}
	}
}

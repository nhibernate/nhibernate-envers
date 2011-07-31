using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class InAuditExpression : IAuditCriterion
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly object[] values;

		public InAuditExpression(IPropertyNameGetter propertyNameGetter, object[] values)
		{
			this.propertyNameGetter = propertyNameGetter;
			this.values = values;
		}

		public void AddToQuery(AuditConfiguration auditCfg, string entityName, QueryBuilder qb, Parameters parameters)
		{
			var propertyName = propertyNameGetter.Get(auditCfg);
			CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);
			parameters.AddWhereWithParams(propertyName, "in (", values, ")");
		}
	}
}

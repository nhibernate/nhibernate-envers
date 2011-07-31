using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class PropertyAuditExpression : IAuditCriterion
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly string otherPropertyName;
		private readonly string op;

		public PropertyAuditExpression(IPropertyNameGetter propertyNameGetter, string otherPropertyName, string op)
		{
			this.propertyNameGetter = propertyNameGetter;
			this.otherPropertyName = otherPropertyName;
			this.op = op;
		}

		public void AddToQuery(AuditConfiguration auditCfg, string entityName, QueryBuilder qb, Parameters parameters)
		{
			var propertyName = propertyNameGetter.Get(auditCfg);
			CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);
			CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, otherPropertyName);
			parameters.AddWhere(propertyName, op, otherPropertyName);
		}
	}
}

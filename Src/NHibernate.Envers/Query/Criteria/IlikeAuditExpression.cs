using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class IlikeAuditExpression : IAuditCriterion
	{
		private readonly IPropertyNameGetter _propertyNameGetter;
		private readonly string _value;

		public IlikeAuditExpression(IPropertyNameGetter propertyNameGetter, string value)
		{
			_propertyNameGetter = propertyNameGetter;
			_value = value;
		}

		public void AddToQuery(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb,
		                       Parameters parameters)
		{
			var propertyName = CriteriaTools.DeterminePropertyName(auditCfg, versionsReader, entityName, _propertyNameGetter);
			CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);
			parameters.AddWhereWithFunction(propertyName, " lower ", " like ", _value.ToLower());
		}
	}
}
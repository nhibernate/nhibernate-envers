using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query.Criteria
{
	public class AuditRelatedId 
	{
		private readonly IPropertyNameGetter _propertyNameGetter;

		public AuditRelatedId(IPropertyNameGetter propertyNameGetter) 
		{
			_propertyNameGetter = propertyNameGetter;
		}

		public IAuditCriterion Eq(object id) 
		{
			return new RelatedAuditExpression(_propertyNameGetter, id, true);
		}

		public IAuditCriterion Ne(object id) 
		{
			return new RelatedAuditExpression(_propertyNameGetter, id, false);
		}
	}
}

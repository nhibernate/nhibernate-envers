using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class RelatedAuditExpression : IAuditCriterion 
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly object id;
		private readonly bool equals;

		public RelatedAuditExpression(IPropertyNameGetter propertyNameGetter, object id, bool equals) 
		{
			this.propertyNameGetter = propertyNameGetter;
			this.id = id;
			this.equals = equals;
		}

		public void AddToQuery(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters) 
		{
			var propertyName = CriteriaTools.DeterminePropertyName(auditCfg, versionsReader, entityName, propertyNameGetter);
			var relatedEntity = CriteriaTools.GetRelatedEntity(auditCfg, entityName, propertyName);

			if (relatedEntity == null) 
			{
				throw new AuditException("This criterion can only be used on a property that is " +
						"a relation to another property.");
			}
			relatedEntity.IdMapper.AddIdEqualsToQuery(parameters, id, null, equals);
		}
	}
}

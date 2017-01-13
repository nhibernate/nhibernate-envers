using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class NullAuditExpression : IAuditCriterion
	{
		private readonly IPropertyNameGetter propertyNameGetter;

		public NullAuditExpression(IPropertyNameGetter propertyNameGetter)
		{
			this.propertyNameGetter = propertyNameGetter;
		}

		public void AddToQuery(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters)
		{
			var propertyName = CriteriaTools.DeterminePropertyName(auditCfg, versionsReader, entityName, propertyNameGetter);
			var relatedEntity = CriteriaTools.GetRelatedEntity(auditCfg, entityName, propertyName);

			if (relatedEntity == null)
			{
				parameters.AddNullRestriction(propertyName, true);
			}
			else
			{
				relatedEntity.IdMapper.AddIdEqualsToQuery(parameters, null, null, true);
			}
		}
	}
}

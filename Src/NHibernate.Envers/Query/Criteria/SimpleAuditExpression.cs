using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class SimpleAuditExpression : IAuditCriterion
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly object value;
		private readonly string op;

		public SimpleAuditExpression(IPropertyNameGetter propertyNameGetter, object value, string op)
		{
			this.propertyNameGetter = propertyNameGetter;
			this.value = value;
			this.op = op;
		}

		public void AddToQuery(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters)
		{
			var propertyName = CriteriaTools.DeterminePropertyName(auditCfg, versionsReader, entityName, propertyNameGetter);

			var relatedEntity = CriteriaTools.GetRelatedEntity(auditCfg, entityName, propertyName);

			if (relatedEntity == null)
			{
				parameters.AddWhereWithParam(propertyName, op, value);
			}
			else
			{
				if (!"=".Equals(op) && !"<>".Equals(op))
				{
					throw new AuditException("This type of operation: " + op + " (" + entityName + "." + propertyName +
							  ") isn't supported and can't be used in queries.");
				}

				var id = relatedEntity.IdMapper.MapToIdFromEntity(value);

				relatedEntity.IdMapper.AddIdEqualsToQuery(parameters, id, null, "=".Equals(op));
			}
		}
	}
}

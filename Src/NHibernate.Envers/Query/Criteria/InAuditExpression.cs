using System.Collections;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class InAuditExpression : IAuditCriterion
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly object[] parameterValues;

		public InAuditExpression(IPropertyNameGetter propertyNameGetter, ICollection values)
		{
			this.propertyNameGetter = propertyNameGetter;
			parameterValues = ensureArray(values);
		}

		public void AddToQuery(AuditConfiguration auditCfg, string entityName, QueryBuilder qb, Parameters parameters)
		{
			var propertyName = propertyNameGetter.Get(auditCfg);
			CriteriaTools.CheckPropertyNotARelation(auditCfg, entityName, propertyName);
			parameters.AddWhereWithParams(propertyName, "in (", parameterValues, ")");
		}

		private static object[] ensureArray(ICollection values)
		{
			var casted = values as object[];
			if (casted != null)
				return casted;

			var array = new object[values.Count];
			values.CopyTo(array, 0);
			return array;
		}
	}
}

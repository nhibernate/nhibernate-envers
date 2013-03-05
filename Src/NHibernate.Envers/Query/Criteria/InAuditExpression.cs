using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class InAuditExpression : IAuditCriterion
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly object[] parameterValues;

		public InAuditExpression(IPropertyNameGetter propertyNameGetter, object[] values)
		{
			this.propertyNameGetter = propertyNameGetter;
			parameterValues = values;
		}

		public void AddToQuery(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters)
		{
			if (parameterValues.Length == 0)
			{
				parameters.AddWhere("1", false, "=", "0", false);
				return;
			}

			var propertyName = CriteriaTools.DeterminePropertyName(auditCfg, versionsReader, entityName, propertyNameGetter);

			var relEntityDesc = CriteriaTools.GetRelatedEntity(auditCfg, entityName, propertyName);
			if (relEntityDesc == null)
			{
				parameters.AddWhereWithParams(propertyName, "in (", parameterValues, ")");				
			}
			else
			{
				//move to IIdMapper when allowing more id sort of queries later
				var dic = new Dictionary<QueryParameterData, IList<object>>();
				for (var i = 0; i < parameterValues.Length; i++)
				{
					var id = relEntityDesc.IdMapper.MapToIdFromEntity(parameterValues[i]);
					var queryParams = relEntityDesc.IdMapper.MapToQueryParametersFromId(id);
					foreach (var queryParam in queryParams)
					{
						if (i == 0)
						{
							dic[queryParam] = new List<object>();
						}
						dic[queryParam].Add(queryParam.Value);
					}
				}
				foreach (var paramNameAndValue in dic)
				{
					parameters.AddWhereWithParams(paramNameAndValue.Key.GetProperty(null), "in (", paramNameAndValue.Value.ToArray(), ")");					
				}
			}
		}
	}
}

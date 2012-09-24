using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Id
{
	[Serializable]
	public abstract class AbstractIdMapper : IIdMapper
	{
		private static Parameters GetParametersToUse(Parameters parameters, ICollection<QueryParameterData> paramDatas)
		{
			return paramDatas.Count > 1 ? parameters.AddSubParameters("and") : parameters;
		}

		public void AddIdsEqualToQuery(Parameters parameters, string prefix1, string prefix2) 
		{
			var paramDatas = MapToQueryParametersFromId(null); 
			var parametersToUse = GetParametersToUse(parameters, paramDatas);

			foreach (var paramData in paramDatas) 
			{
				parametersToUse.AddWhere(paramData.GetProperty(prefix1), false, "=", paramData.GetProperty(prefix2), false);
			}
		}

		public void AddIdsEqualToQuery(Parameters parameters, string prefix1, IIdMapper mapper2, string prefix2)
		{
			var paramDatas1 = MapToQueryParametersFromId(null);
			var paramDatas2 = mapper2.MapToQueryParametersFromId(null); 
			var parametersToUse = GetParametersToUse(parameters, paramDatas1);

			for (var i = 0; i < paramDatas1.Count; i++)
			{
				var paramData1 = paramDatas1[i];
				var paramData2 = paramDatas2[i];
				parametersToUse.AddWhere(paramData1.GetProperty(prefix1), false, "=", paramData2.GetProperty(prefix2), false);
			}
		}

		public void AddIdEqualsToQuery(Parameters parameters, object id, string prefix, bool equals) 
		{
			var paramDatas = MapToQueryParametersFromId(id);
			var parametersToUse = GetParametersToUse(parameters, paramDatas);

			foreach (var paramData in paramDatas) 
			{
				if (paramData.Value == null)
				{
					handleNullValue(parametersToUse, paramData.GetProperty(prefix), equals);
				}
				else
				{
					parametersToUse.AddWhereWithParam(paramData.GetProperty(prefix), equals ? "=" : "<>", paramData.Value);					
				}
			}
		}

		private static void handleNullValue(Parameters parameters, string propertyName, bool equals)
		{
			if (equals)
			{
				parameters.AddNullRestriction(propertyName, equals);
			}
			else
			{
				parameters.AddNotNullRestriction(propertyName, equals);
			}
		}

		public void AddNamedIdEqualsToQuery(Parameters parameters, string prefix, bool equals) 
		{
			var paramDatas = MapToQueryParametersFromId(null);
			var parametersToUse = GetParametersToUse(parameters, paramDatas);

			foreach (var paramData in paramDatas) 
			{
				parametersToUse.AddWhereWithNamedParam(paramData.GetProperty(prefix), equals ? "=" : "<>", paramData.QueryParameterName);
			}
		}

		public abstract IList<QueryParameterData> MapToQueryParametersFromId(object obj);
		public abstract void MapToMapFromId(IDictionary<string, object> data, object obj);
		public abstract void MapToMapFromEntity(IDictionary<string, object> data, object obj);
		public abstract bool MapToEntityFromMap(object obj, IDictionary data);
		public abstract object MapToIdFromEntity(object data);
		public abstract object MapToIdFromMap(IDictionary data);
		public abstract IIdMapper PrefixMappedProperties(string prefix);
	}
}

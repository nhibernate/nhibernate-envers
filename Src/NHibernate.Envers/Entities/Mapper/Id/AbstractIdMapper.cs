using System;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Id
{
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
				parametersToUse.AddWhere(paramData.getProperty(prefix1), false, "=", paramData.getProperty(prefix2), false);
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
				parametersToUse.AddWhere(paramData1.getProperty(prefix1), false, "=", paramData2.getProperty(prefix2), false);
			}
		}

		public void AddIdEqualsToQuery(Parameters parameters, object id, string prefix, bool equals) 
		{
			var paramDatas = MapToQueryParametersFromId(id);
			var parametersToUse = GetParametersToUse(parameters, paramDatas);

			foreach (var paramData in paramDatas) 
			{
				parametersToUse.AddWhereWithParam(paramData.getProperty(prefix) , equals ? "=" : "<>", paramData.getValue());
			}
		}

		public void AddNamedIdEqualsToQuery(Parameters parameters, String prefix, bool equals) 
		{
			var paramDatas = MapToQueryParametersFromId(null);
			var parametersToUse = GetParametersToUse(parameters, paramDatas);

			foreach (var paramData in paramDatas) 
			{
				parametersToUse.AddWhereWithNamedParam(paramData.getProperty(prefix), equals ? "=" : "<>", paramData.GetQueryParameterName());
			}
		}

		public abstract IList<QueryParameterData> MapToQueryParametersFromId(object obj);
		public abstract void MapToMapFromId(IDictionary<string, object> data, object obj);
		public abstract void MapToMapFromEntity(IDictionary<string, object> data, object obj);
		public abstract void MapToEntityFromMap(object obj, IDictionary<string, object> data);
		public abstract object MapToIdFromEntity(object data);
		public abstract object MapToIdFromMap(IDictionary<string, object> data);
		public abstract IIdMapper PrefixMappedProperties(string prefix);
	}
}

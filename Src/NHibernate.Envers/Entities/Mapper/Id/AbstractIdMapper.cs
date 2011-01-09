using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools.Query;
using NHibernate.Envers.Entities.Mapper.Id;

/**
 * @author Catalina Panait, port of Envers omonyme class by Adam Warski (adam at warski dot org)
 */
namespace NHibernate.Envers.Entities.Mapper.Id
{
    public abstract class AbstractIdMapper:  IIdMapper
    {
        private static Parameters GetParametersToUse(Parameters parameters, ICollection<QueryParameterData> paramDatas)
        {
            return paramDatas.Count > 1 ? parameters.AddSubParameters("and") : parameters;
        }

        public void AddIdsEqualToQuery(Parameters parameters, String prefix1, String prefix2) {
        IList<QueryParameterData> paramDatas = MapToQueryParametersFromId(null); 

        Parameters parametersToUse = GetParametersToUse(parameters, paramDatas);

        foreach (QueryParameterData paramData in paramDatas) {
            parametersToUse.AddWhere(paramData.getProperty(prefix1), false, "=", paramData.getProperty(prefix2), false);
        }
    }

        public void AddIdsEqualToQuery(Parameters parameters, String prefix1, IIdMapper mapper2, String prefix2)
        {
            var paramDatas1 = MapToQueryParametersFromId(null);
            var paramDatas2 = mapper2.MapToQueryParametersFromId(null); 

            var parametersToUse = GetParametersToUse(parameters, paramDatas1);

        	var paramData2 = paramDatas2[0];
        	foreach (var paramData1 in paramDatas1)
        	{
				parametersToUse.AddWhere(paramData1.getProperty(prefix1), false, "=", paramData2.getProperty(prefix2), false);        		
        	}
        }

        public void AddIdEqualsToQuery(Parameters parameters, Object id, String prefix, bool equals) {
        IList<QueryParameterData> paramDatas = MapToQueryParametersFromId(id); 

        Parameters parametersToUse = GetParametersToUse(parameters, paramDatas);

        foreach (QueryParameterData paramData in paramDatas) {
            parametersToUse.AddWhereWithParam(paramData.getProperty(prefix) , equals ? "=" : "<>", paramData.getValue());
        }
    }

        public void AddNamedIdEqualsToQuery(Parameters parameters, String prefix, bool equals) {
        IList<QueryParameterData> paramDatas = MapToQueryParametersFromId(null); 

        Parameters parametersToUse = GetParametersToUse(parameters, paramDatas);

        foreach (QueryParameterData paramData in paramDatas) {
            parametersToUse.AddWhereWithNamedParam(paramData.getProperty(prefix), equals ? "=" : "<>", paramData.GetQueryParameterName());
        }
    }

        public abstract IList<QueryParameterData> MapToQueryParametersFromId(Object obj);

        #region IIdMapper Members

        public abstract void MapToMapFromId(IDictionary<string, object> data, object obj);

        public abstract void MapToMapFromEntity(IDictionary<string, object> data, object obj);

        public abstract void MapToEntityFromMap(object obj, IDictionary<string, object> data);

        public abstract object MapToIdFromEntity(object data);

        public abstract object MapToIdFromMap(IDictionary<string, object> data);

        public abstract IIdMapper PrefixMappedProperties(string prefix);

        #endregion
    }
}

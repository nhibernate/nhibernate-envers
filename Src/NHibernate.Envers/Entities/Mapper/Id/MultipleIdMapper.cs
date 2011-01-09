using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Exceptions;
using NHibernate.Util;

namespace NHibernate.Envers.Entities.Mapper.Id
{

    /**
     * @author Catalina Panait, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class MultipleIdMapper : AbstractCompositeIdMapper, ISimpleIdMapperBuilder
    {
        public MultipleIdMapper(System.Type compositeIdClass)
            : base(compositeIdClass)
            {}

        public override void MapToMapFromId(IDictionary<String, Object> data, Object obj)
        {
            foreach (IIdMapper idMapper in ids.Values)
            { 
                idMapper.MapToMapFromEntity(data, obj);
        }
    }

        public override void MapToMapFromEntity(IDictionary<String, Object> data, Object obj)
        {
           MapToMapFromId(data, obj);
        }

        public override void MapToEntityFromMap(Object obj, IDictionary<String, Object> data) {
        foreach (IIdMapper idMapper in ids.Values) {
            idMapper.MapToEntityFromMap(obj,data);
        }
    }

        public override IIdMapper PrefixMappedProperties(String prefix)
        {
        MultipleIdMapper ret = new MultipleIdMapper(compositeIdClass);

        foreach (PropertyData propertyData in ids.Keys) {
            String propertyName = propertyData.Name;
            ret.ids.Add(propertyData, new SingleIdMapper(new PropertyData(prefix + propertyName, propertyData)));
        }

        return ret;
    }

        public override Object MapToIdFromEntity(Object data)
        {
        if (data == null) {
            return null;
        }

        Object ret;
        try {
            ret = Activator.CreateInstance(compositeIdClass );
            //ret = Thread.currentThread().getContextClassLoader().loadClass(compositeIdClass).newInstance();
        } catch (Exception e) {
            throw new AuditException(e);
        }

        foreach (SingleIdMapper mapper in ids.Values) {
            mapper.MapToEntityFromEntity(ret, data);
        }

        return ret;
    }

        public override IList<QueryParameterData> MapToQueryParametersFromId(Object obj)
        {
            //Simon 27/06/2010 - era new LinkedHashMap
            IDictionary<String, Object> data = new Dictionary<String, Object>();
            MapToMapFromId(data, obj);

            IList<QueryParameterData> ret = new List<QueryParameterData>();

            foreach (KeyValuePair<String, Object> propertyData in data)
            {
                ret.Add(new QueryParameterData(propertyData.Key, propertyData.Value));
            }


            return ret;
        }


    }
}

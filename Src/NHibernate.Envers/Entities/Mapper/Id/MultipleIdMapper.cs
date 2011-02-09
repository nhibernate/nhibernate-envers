using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Exceptions;

namespace NHibernate.Envers.Entities.Mapper.Id
{
    public class MultipleIdMapper : AbstractCompositeIdMapper, ISimpleIdMapperBuilder
    {
        public MultipleIdMapper(System.Type compositeIdClass)
            : base(compositeIdClass)
            {}

        public override void MapToMapFromId(IDictionary<string, object> data, object obj)
        {
            foreach (IIdMapper idMapper in ids.Values)
            { 
                idMapper.MapToMapFromEntity(data, obj);
			}
		}

        public override void MapToMapFromEntity(IDictionary<string, object> data, object obj)
        {
           MapToMapFromId(data, obj);
        }

        public override void MapToEntityFromMap(object obj, IDictionary data) 
		{
			foreach (IIdMapper idMapper in ids.Values) 
			{
				idMapper.MapToEntityFromMap(obj,data);
			}
		}

        public override IIdMapper PrefixMappedProperties(string prefix)
        {
			var ret = new MultipleIdMapper(compositeIdClass);

			foreach (var propertyData in ids.Keys) 
			{
				var propertyName = propertyData.Name;
				ret.ids.Add(propertyData, new SingleIdMapper(new PropertyData(prefix + propertyName, propertyData)));
			}

			return ret;
		}

        public override object MapToIdFromEntity(object data)
        {
			if (data == null) 
			{
				return null;
			}

			object ret;
			try 
			{
				ret = Activator.CreateInstance(compositeIdClass );
			} 
			catch (Exception e) 
			{
				throw new AuditException(e);
			}

			foreach (SingleIdMapper mapper in ids.Values) 
			{
				mapper.MapToEntityFromEntity(ret, data);
			}

			return ret;
		}

        public override IList<QueryParameterData> MapToQueryParametersFromId(object obj)
        {
            //Simon 27/06/2010 - era new LinkedHashMap
            IDictionary<string, object> data = new Dictionary<string, object>();
            MapToMapFromId(data, obj);

            IList<QueryParameterData> ret = new List<QueryParameterData>();

            foreach (KeyValuePair<string, object> propertyData in data)
            {
                ret.Add(new QueryParameterData(propertyData.Key, propertyData.Value));
            }

            return ret;
        }
    }
}

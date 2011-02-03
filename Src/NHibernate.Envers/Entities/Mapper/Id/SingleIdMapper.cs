using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Proxy;

namespace NHibernate.Envers.Entities.Mapper.Id
{
	public class SingleIdMapper : AbstractIdMapper, ISimpleIdMapperBuilder 
	{
		private PropertyData propertyData;

		public SingleIdMapper() {}

		public SingleIdMapper(PropertyData propertyData) 
		{
			this.propertyData = propertyData;
		}

		public void Add(PropertyData propertyData) 
		{
			if (this.propertyData != null) 
			{
				throw new AuditException("Only one property can be added!");
			}

			this.propertyData = propertyData;
		}

		public override void MapToEntityFromMap(object obj, IDictionary data)
		{
			if (data == null || obj == null) 
            {
				return;
			}
			var setter = ReflectionTools.GetSetter(obj.GetType(), propertyData);
			setter.Set(obj, data[propertyData.Name]);
		}

		public override object MapToIdFromMap(IDictionary data)
		{
		    return data == null ? null : data[propertyData.Name];
		}

	    public override object MapToIdFromEntity(object data)
		{
			if (data == null) 
            {
				return null;
			}

			var proxy = data as INHibernateProxy;

			if(proxy != null) 
			{
				return proxy.HibernateLazyInitializer.Identifier;
			}
			var getter = ReflectionTools.GetGetter(data.GetType(), propertyData);
			return getter.Get(data);
		}

		public override void MapToMapFromId(IDictionary<string, object> data, object obj)
		{
			if (data != null) 
            {
				data.Add(propertyData.Name, obj);
			}
		}

		public override void MapToMapFromEntity(IDictionary<string, object> data, object obj)
		{
			if (obj == null) 
            {
				data.Add(propertyData.Name,null);
			} 
            else
			{
			    var proxy = obj as INHibernateProxy;
				if(proxy!=null) 
                {
					data.Add(propertyData.Name, proxy.HibernateLazyInitializer.Identifier);
				} 
                else 
                {
                    var getter = ReflectionTools.GetGetter(obj.GetType(), propertyData);
                    data.Add(propertyData.Name, getter.Get(obj));
				}
			}
		}

		public void MapToEntityFromEntity(object objTo, object objFrom) 
        {
			if (objTo == null || objFrom == null) 
            {
				return;
			}
		    var getter = ReflectionTools.GetGetter(objFrom.GetType(), propertyData);
		    var setter = ReflectionTools.GetSetter(objTo.GetType(), propertyData);
            setter.Set(objTo, getter.Get(objFrom));
		}

		public override IIdMapper PrefixMappedProperties(string prefix) 
        {
			return new SingleIdMapper(new PropertyData(prefix + propertyData.Name, propertyData));
		}

		public override IList<QueryParameterData> MapToQueryParametersFromId(object obj)
		{
			IList<QueryParameterData> ret = new List<QueryParameterData> {new QueryParameterData(propertyData.Name, obj)};

		    return ret;
		}
	}
}

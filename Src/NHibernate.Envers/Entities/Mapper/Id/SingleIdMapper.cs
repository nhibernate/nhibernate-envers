using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Proxy;

namespace NHibernate.Envers.Entities.Mapper.Id
{
	[Serializable]
	public class SingleIdMapper : AbstractIdMapper, ISimpleIdMapperBuilder
	{
		private PropertyData _propertyData;

		public SingleIdMapper() { }

		public SingleIdMapper(PropertyData propertyData)
		{
			_propertyData = propertyData;
		}

		public void Add(PropertyData propertyData)
		{
			if (_propertyData != null)
			{
				throw new AuditException("Only one property can be added!");
			}

			_propertyData = propertyData;
		}

		public override bool MapToEntityFromMap(object obj, IDictionary data)
		{
			if (data == null || obj == null)
				return false;
			var value = data[_propertyData.Name];
			if (value == null)
				return false;

			var setter = ReflectionTools.GetSetter(obj.GetType(), _propertyData);
			setter.Set(obj, value);
			return true;
		}

		public override object MapToIdFromMap(IDictionary data)
		{
			return data == null ? null : data[_propertyData.Name];
		}

		public override object MapToIdFromEntity(object data)
		{
			if (data == null)
			{
				return null;
			}

			var proxy = data as INHibernateProxy;

			if (proxy != null)
			{
				return proxy.HibernateLazyInitializer.Identifier;
			}
			var getter = ReflectionTools.GetGetter(data.GetType(), _propertyData);
			return getter.Get(data);
		}

		public override void MapToMapFromId(IDictionary<string, object> data, object obj)
		{
			if (data != null)
			{
				data.Add(_propertyData.Name, obj);
			}
		}

		public override void MapToMapFromEntity(IDictionary<string, object> data, object obj)
		{
			if (obj == null)
			{
				data.Add(_propertyData.Name, null);
			}
			else
			{
				var proxy = obj as INHibernateProxy;
				if (proxy != null)
				{
					data.Add(_propertyData.Name, proxy.HibernateLazyInitializer.Identifier);
				}
				else
				{
					var getter = ReflectionTools.GetGetter(obj.GetType(), _propertyData);
					data.Add(_propertyData.Name, getter.Get(obj));
				}
			}
		}

		public override IIdMapper PrefixMappedProperties(string prefix)
		{
			return new SingleIdMapper(new PropertyData(prefix + _propertyData.Name, _propertyData));
		}

		public override IList<QueryParameterData> MapToQueryParametersFromId(object obj)
		{
			return new List<QueryParameterData> { new QueryParameterData(_propertyData.Name, obj) };
		}
	}
}

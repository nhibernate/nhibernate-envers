using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper.Id
{
	[Serializable]
	public class EmbeddedIdMapper : AbstractCompositeIdMapper
	{
		private readonly PropertyData _idPropertyData;

		public EmbeddedIdMapper(PropertyData idPropertyData, System.Type compositeIdClass)
								: base(compositeIdClass)
		{
			_idPropertyData = idPropertyData;
		}

		public override void MapToMapFromId(IDictionary<string, object> data, object obj)
		{
			foreach (var idMapper in Ids.Values) 
			{
				idMapper.MapToMapFromEntity(data, obj);
			}
		}

		public override void MapToMapFromEntity(IDictionary<string, object> data, object obj)
		{
			if (obj == null)
			{
				return;
			}
			var getter = ReflectionTools.GetGetter(obj.GetType(), _idPropertyData);
			MapToMapFromId(data, getter.Get(obj));
		}

		public override bool MapToEntityFromMap(object obj, IDictionary data)
		{
			if (data == null || obj == null) 
				return false;

			var objType = obj.GetType();
			var getter = ReflectionTools.GetGetter(objType, _idPropertyData);
			var setter = ReflectionTools.GetSetter(objType, _idPropertyData);

			try 
			{
				var subObj = ReflectionTools.CreateInstanceByDefaultConstructor(getter.ReturnType);
				var ret = true;
				foreach(var idMapper in Ids.Values)
				{
					ret &= idMapper.MapToEntityFromMap(subObj, data);
				}
				if (ret)
				{
					setter.Set(obj, subObj);
				}
				return ret;
			} 
			catch (Exception e) 
			{
				throw new AuditException("Cannot create instance of type " + getter.ReturnType, e);
			}
		}

		public override IIdMapper PrefixMappedProperties(string prefix)
		{
			var ret = new EmbeddedIdMapper(_idPropertyData, CompositeIdClass);

			foreach (var propertyData in Ids.Keys) 
			{
				var propertyName = propertyData.Name;
				ret.Ids.Add(propertyData, new SingleIdMapper(new PropertyData(prefix + propertyName, propertyData)));
			}

			return ret;
		}

		public override object MapToIdFromEntity(object data)
		{
			if (data == null)
			{
				return null;
			}
			var getter = ReflectionTools.GetGetter(data.GetType(), _idPropertyData);
			return getter.Get(data);
		}

		public override IList<QueryParameterData> MapToQueryParametersFromId(object obj) 
		{
			var data = new Dictionary<string, object>();
			MapToMapFromId(data, obj);

			return data.Select(propertyData => new QueryParameterData(propertyData.Key, propertyData.Value)).ToList();
		}
	}
}

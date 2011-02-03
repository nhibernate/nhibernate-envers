using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Util;
using NHibernate.Envers.Exceptions;

namespace NHibernate.Envers.Entities.Mapper.Id
{
	public class EmbeddedIdMapper : AbstractCompositeIdMapper, ISimpleIdMapperBuilder
	{
		private readonly PropertyData idPropertyData;

		public EmbeddedIdMapper(PropertyData idPropertyData, System.Type compositeIdClass)
								: base(compositeIdClass)
		{
			this.idPropertyData = idPropertyData;
		}

		public override void MapToMapFromId(IDictionary<string, object> data, object obj)
		{
			foreach (var idMapper in ids.Values) 
			{
				idMapper.MapToMapFromEntity(data, obj);
			}
		}

		public override void MapToMapFromEntity(IDictionary<string, object> data, Object obj)
		{
			if (obj == null)
			{
				return;
			}
			var getter = ReflectionTools.GetGetter(obj.GetType(), idPropertyData);
			MapToMapFromId(data, getter.Get(obj));
		}

		public override void MapToEntityFromMap(object obj, IDictionary data)
		{
			if (data == null || obj == null) 
			{
				return;
			}
			var objType = obj.GetType();
			var getter = ReflectionTools.GetGetter(objType, idPropertyData);
			var setter = ReflectionTools.GetSetter(objType, idPropertyData);

			try 
			{
				var subObj = ReflectHelper.GetDefaultConstructor(getter.ReturnType).Invoke(null); 
				setter.Set(obj, subObj);

				foreach(var idMapper in ids.Values) 
				{
					idMapper.MapToEntityFromMap(subObj, data);
				}
			} 
			catch (Exception e) 
			{
				throw new AuditException(e);
			}
		}

		public override IIdMapper PrefixMappedProperties(string prefix)
		{
			var ret = new EmbeddedIdMapper(idPropertyData, compositeIdClass);

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
			var getter = ReflectionTools.GetGetter(data.GetType(), idPropertyData);
			return getter.Get(data);
		}

		public override IList<QueryParameterData> MapToQueryParametersFromId(object obj) 
		{
			//Simon 27/06/2010 - era LinkedHashMap
			var data = new Dictionary<String, Object>();
			MapToMapFromId(data, obj);

			var ret = new List<QueryParameterData>();

			foreach (var propertyData in data) 
			{
				ret.Add(new QueryParameterData(propertyData.Key, propertyData.Value));
			}

			return ret;
		}

	}
}

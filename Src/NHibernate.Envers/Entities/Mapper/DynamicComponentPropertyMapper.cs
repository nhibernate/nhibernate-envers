using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper
{
	public class DynamicComponentPropertyMapper : IPropertyMapper, ICompositeMapperBuilder
	{
		private readonly PropertyData _propertyData;
		private readonly MultiPropertyMapper _delegate;

		public DynamicComponentPropertyMapper(PropertyData propertyData)
		{
			_propertyData = propertyData;
			_delegate = new MultiPropertyMapper();
		}

		public void Add(PropertyData propertyData)
		{
			_delegate.Add(propertyData);
		}

		public ICompositeMapperBuilder AddComponent(PropertyData propertyData, string componentClassName)
		{
			return _delegate.AddComponent(propertyData, componentClassName);
		}

		public void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper)
		{
			_delegate.AddComposite(propertyData, propertyMapper);
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data,
									   object newObj, object oldObj)
		{
			var ret = false;
			var properties = _delegate.Properties;
			foreach (var propertyData in properties.Keys)
			{
				var newValue = newObj == null ? null : ((IDictionary)newObj)[propertyData.BeanName];
				var oldValue = oldObj == null ? null : ((IDictionary)oldObj)[propertyData.BeanName];

				ret |= properties[propertyData].MapToMapFromEntity(session, data, newValue, oldValue);
			}
			return ret;
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data,
									   object primaryKey, IAuditReaderImplementor versionsReader, long revision)
		{
			if (data == null || obj == null)
			{
				return;
			}

			var setter = ReflectionTools.GetSetter(obj.GetType(), _propertyData);

			// If all properties are null and single, then the component has to be null also.
			var allNullAndSingle = true;
			foreach (var property in _delegate.Properties)
			{
				if (data[property.Key.Name] != null || !(property.Value is SinglePropertyMapper))
				{
					allNullAndSingle = false;
					break;
				}
			}

			// And we don't have to set anything on the object - the default value is null
			if (!allNullAndSingle)
			{
				var subObj = new Dictionary<string, object>();
				foreach (var propertyData in _delegate.Properties.Keys)
				{
					subObj[propertyData.BeanName] = data[propertyData.Name];
				}
				setter.Set(obj, subObj);
			}
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(string referencingPropertyName,
																		IPersistentCollection newColl,
																		object oldColl,
																		object id)
		{
			return _delegate.MapCollectionChanges(referencingPropertyName, newColl, oldColl, id);
		}
	}
}
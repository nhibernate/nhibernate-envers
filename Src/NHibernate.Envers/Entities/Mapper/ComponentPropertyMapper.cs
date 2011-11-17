using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper
{
	public class ComponentPropertyMapper : IPropertyMapper, ICompositeMapperBuilder
	{
		private readonly PropertyData propertyData;
		private readonly MultiPropertyMapper _delegate;
		private readonly string componentClassName;

		public ComponentPropertyMapper(PropertyData propertyData, string componentClassName)
		{
			this.propertyData = propertyData;
			_delegate = new MultiPropertyMapper();
			this.componentClassName = componentClassName;
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
			return _delegate.MapToMapFromEntity(session, data, newObj, oldObj);
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data,
										object primaryKey, IAuditReaderImplementor versionsReader, long revision)
		{
			if (data == null || obj == null)
			{
				return;
			}

			if (propertyData.BeanName == null)
			{
				// If properties are not encapsulated in a component but placed directly in a class
				// (e.g. by applying <properties> tag).
				_delegate.MapToEntityFromMap(verCfg, obj, data, primaryKey, versionsReader, revision);
				return;
			}

			var setter = ReflectionTools.GetSetter(obj.GetType(), propertyData);

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

			if (allNullAndSingle)
			{
				// single property, but default value need not be null, so we'll set it to null anyway 
				setter.Set(obj, null);	
			}
			else
			{
				var componentType = Toolz.ResolveDotnetType(componentClassName);
				var subObj = ReflectionTools.CreateInstanceByDefaultConstructor(componentType);
				_delegate.MapToEntityFromMap(verCfg, subObj, data, primaryKey, versionsReader, revision);
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

using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Collection;
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

			// And we don't have to set anything on the object - the default value is null
			if (!allNullAndSingle) 
			{
				try 
				{
					var subObj = Activator.CreateInstance(Toolz.ResolveDotnetType(componentClassName));
					_delegate.MapToEntityFromMap(verCfg, subObj, data, primaryKey, versionsReader, revision);
					setter.Set(obj, subObj);
				} 
				catch (Exception e) 
				{
					throw new AuditException("Cannot create instance of type " + componentClassName, e);
				}
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

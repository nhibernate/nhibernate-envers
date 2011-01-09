using System;
using System.Collections.Generic;
using NHibernate.Envers.Tools;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.Entities.Mapper
{
	public class MultiPropertyMapper : IExtendedPropertyMapper 
	{
		public IDictionary<PropertyData, IPropertyMapper> Properties { get; protected set; }
		private readonly IDictionary<String, PropertyData> propertyDatas;

		public MultiPropertyMapper() 
		{
			Properties = new Dictionary<PropertyData, IPropertyMapper>();
			propertyDatas = new Dictionary<String, PropertyData>();
		}

		public void Add(PropertyData propertyData) 
		{
			var single = new SinglePropertyMapper();
			single.Add(propertyData);
			Properties.Add(propertyData, single);
			propertyDatas.Add(propertyData.Name, propertyData);
		}

		public ICompositeMapperBuilder AddComponent(PropertyData propertyData, String componentClassName) 
		{
			if (Properties.ContainsKey(propertyData)) 
			{
				// This is needed for second pass to work properly in the components mapper
				return (ICompositeMapperBuilder) Properties[propertyData];
			}

			var componentMapperBuilder = new ComponentPropertyMapper(propertyData, componentClassName);
			AddComposite(propertyData, componentMapperBuilder);

			return componentMapperBuilder;
		}

		public void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper) 
		{
			Properties.Add(propertyData, propertyMapper);
			propertyDatas.Add(propertyData.Name, propertyData);
		}

		private Object GetAtIndexOrNull(Object[] array, int index) { return array == null ? null : array[index]; }

		public bool Map(ISessionImplementor session, IDictionary<String, Object> data, String[] propertyNames, 
						Object[] newState, Object[] oldState) {
			bool ret = false;
			for (int i=0; i<propertyNames.Length; i++) {
				String propertyName = propertyNames[i];

				if (propertyDatas.ContainsKey(propertyName)) {
					ret |= Properties[propertyDatas[propertyName]].MapToMapFromEntity(session, data,
							GetAtIndexOrNull(newState, i),
							GetAtIndexOrNull(oldState, i));
				}
			}

			return ret;
		}

		public bool MapToMapFromEntity(ISessionImplementor session, 
										IDictionary<string, object> data,
										object newObj, 
										object oldObj)
		{
			var ret = false;
			foreach (var propertyData in Properties.Keys) 
			{
				IGetter getter;
				if (newObj != null) 
				{
					getter = ReflectionTools.GetGetter(newObj.GetType(), propertyData);
				} 
				else if (oldObj != null) 
				{
					getter = ReflectionTools.GetGetter(oldObj.GetType(), propertyData);
				} 
				else 
				{
					return false;
				}

				ret |= Properties[propertyData].MapToMapFromEntity(session, data,
						newObj == null ? null : getter.Get(newObj),
						oldObj == null ? null : getter.Get(oldObj));
			}

			return ret;
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary<string, object> data,
									   object primaryKey, IAuditReaderImplementor versionsReader, long revision) 
		{
			foreach (var mapper in Properties.Values) 
			{
				mapper.MapToEntityFromMap(verCfg, obj, data, primaryKey, versionsReader, revision);
			}
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(string referencingPropertyName,
																		IPersistentCollection newColl,
																		object oldColl,
																		object id) 
		{
			// Name of the property, to which we will delegate the mapping.
			string delegatePropertyName;

			// Checking if the property name doesn't reference a collection in a component - then the name will containa a .
			var dotIndex = referencingPropertyName.IndexOf('.');
			if (dotIndex != -1) 
			{
				// Computing the name of the component
				var componentName = referencingPropertyName.Substring(0, dotIndex);
				// And the name of the property in the component
				var propertyInComponentName = MappingTools.createComponentPrefix(componentName)
						+ referencingPropertyName.Substring(dotIndex+1);

				// We need to get the mapper for the component.
				referencingPropertyName = componentName;
				// As this is a component, we delegate to the property in the component.
				delegatePropertyName = propertyInComponentName;
			} 
			else 
			{
				// If this is not a component, we delegate to the same property.
				delegatePropertyName = referencingPropertyName;
			}

			PropertyData propertyData;
			if (propertyDatas.TryGetValue(referencingPropertyName, out propertyData))
			{
				IPropertyMapper propertyMapper;
				if(Properties.TryGetValue(propertyData, out propertyMapper))
				{
					return propertyMapper.MapCollectionChanges(delegatePropertyName, newColl, oldColl, id);
				}
			}
			return null;
		}
	}
}

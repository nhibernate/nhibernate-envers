using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.Entities.Mapper
{
	[Serializable]
	public class MultiPropertyMapper : IExtendedPropertyMapper 
	{
		public MultiPropertyMapper() 
		{
			Properties = new Dictionary<PropertyData, IPropertyMapper>();
			PropertyDatas = new Dictionary<string, PropertyData>();
		}

		public IDictionary<PropertyData, IPropertyMapper> Properties { get; private set; }
		private IDictionary<string, PropertyData> PropertyDatas { get; set; }

		public void Add(PropertyData propertyData) 
		{
			var single = new SinglePropertyMapper();
			single.Add(propertyData);
			Properties.Add(propertyData, single);
			PropertyDatas.Add(propertyData.Name, propertyData);
		}

		public ICompositeMapperBuilder AddComponent(PropertyData propertyData, string componentClassName) 
		{
			if (Properties.ContainsKey(propertyData)) 
			{
				// This is needed for second pass to work properly in the components mapper
				return (ICompositeMapperBuilder) Properties[propertyData];
			}

			ICompositeMapperBuilder componentMapperBuilder;
			//todo: rk - not really reliable I think!
			if(componentClassName==null)
			{
				componentMapperBuilder = new DynamicComponentPropertyMapper(propertyData);
			}
			else
			{
				componentMapperBuilder = new ComponentPropertyMapper(propertyData, componentClassName);				
			}

			AddComposite(propertyData, (IPropertyMapper) componentMapperBuilder);

			return componentMapperBuilder;
		}

		public void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper) 
		{
			Properties.Add(propertyData, propertyMapper);
			PropertyDatas.Add(propertyData.Name, propertyData);
		}

		private static object getAtIndexOrNull(IList<object> array, int index)
		{
			return array == null ? null : array[index];
		}

		public bool Map(ISessionImplementor session, IDictionary<string, object> data, string[] propertyNames, 
						object[] newState, object[] oldState) 
		{
			var ret = false;
			for (var i=0; i<propertyNames.Length; i++) 
			{
				var propertyName = propertyNames[i];

				if (PropertyDatas.ContainsKey(propertyName)) 
				{
					var propertyMapper = Properties[PropertyDatas[propertyName]];
					var newObj = getAtIndexOrNull(newState, i);
					var oldObj = getAtIndexOrNull(oldState, i);
					ret |= propertyMapper.MapToMapFromEntity(session, data, newObj, oldObj);
					propertyMapper.MapModifiedFlagsToMapFromEntity(session, data, newObj, oldObj);
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

		public void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
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
					return;
				}
				Properties[propertyData].MapModifiedFlagsToMapFromEntity(session, data,
				                                                         newObj == null ? null : getter.Get(newObj),
				                                                         oldObj == null ? null : getter.Get(oldObj));
			}
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data,
									   object primaryKey, IAuditReaderImplementor versionsReader, long revision) 
		{
			foreach (var mapper in Properties.Values) 
			{
				mapper.MapToEntityFromMap(verCfg, obj, data, primaryKey, versionsReader, revision);
			}
		}

		private Tuple<IPropertyMapper, string> mapperAndDelegatePropertyName(string referencingPropertyName)
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
				var propertyInComponentName = MappingTools.CreateComponentPrefix(componentName)
						+ referencingPropertyName.Substring(dotIndex + 1);

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
			var propertyMapper = propertyMapperKey(referencingPropertyName);
			return propertyMapper == null ? null : new Tuple<IPropertyMapper, string>(propertyMapper, delegatePropertyName);
		}

		private IPropertyMapper propertyMapperKey(string referencingPropertyName)
		{
			PropertyData propertyData;
			if (PropertyDatas.TryGetValue(referencingPropertyName, out propertyData))
			{
				IPropertyMapper propertyMapper;
				if (Properties.TryGetValue(propertyData, out propertyMapper))
				{
					return propertyMapper;
				}
			}
			return null;
		}

		public void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data)
		{
			var pair = mapperAndDelegatePropertyName(collectionPropertyName);
			if (pair != null)
			{
				pair.Item1.MapModifiedFlagsToMapForCollectionChange(pair.Item2, data);
			}
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(ISessionImplementor session, 
																		string referencingPropertyName,
																		IPersistentCollection newColl,
																		object oldColl,
																		object id)
		{
			var pair = mapperAndDelegatePropertyName(referencingPropertyName);
			return pair == null ? 
								null :
								pair.Item1.MapCollectionChanges(session, pair.Item2, newColl, oldColl, id);
		}
	}
}

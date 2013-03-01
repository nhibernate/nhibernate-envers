using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper
{
	/// <summary>
	/// A mapper which maps from a parent mapper and a "main" one, but adds only to the "main". The "main" mapper
	/// should be the mapper of the subclass.
	/// </summary>
	[Serializable]
	public class SubclassPropertyMapper : IExtendedPropertyMapper 
	{
		private readonly IExtendedPropertyMapper main;
		private readonly IExtendedPropertyMapper parentMapper;

		public SubclassPropertyMapper(IExtendedPropertyMapper main, IExtendedPropertyMapper parentMapper) 
		{
			this.main = main;
			this.parentMapper = parentMapper;
		}

		public bool Map(ISessionImplementor session, IDictionary<string, object> data, string[] propertyNames, object[] newState, object[] oldState) 
		{
			var parentDiffs = parentMapper.Map(session, data, propertyNames, newState, oldState);
			var mainDiffs = main.Map(session, data, propertyNames, newState, oldState);

			return parentDiffs || mainDiffs;
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj) 
		{
			var parentDiffs = parentMapper.MapToMapFromEntity(session, data, newObj, oldObj);
			var mainDiffs = main.MapToMapFromEntity(session, data, newObj, oldObj);

			return parentDiffs || mainDiffs;
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey, IAuditReaderImplementor versionsReader, long revision) 
		{
			parentMapper.MapToEntityFromMap(verCfg, obj, data, primaryKey, versionsReader, revision);
			main.MapToEntityFromMap(verCfg, obj, data, primaryKey, versionsReader, revision);
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(ISessionImplementor session, 
																		string referencingPropertyName,
																		IPersistentCollection newColl, 
																		object oldColl,
																		object id) 
		{
			var parentCollectionChanges = parentMapper.MapCollectionChanges(
					session, referencingPropertyName, newColl, oldColl, id);

			var mainCollectionChanges = main.MapCollectionChanges(
					session, referencingPropertyName, newColl, oldColl, id);

			if (parentCollectionChanges == null) 
			{
				return mainCollectionChanges;
			}
			if(mainCollectionChanges != null) 
			{
				parentCollectionChanges.Concat(mainCollectionChanges);
			}
			return parentCollectionChanges;
		}

		public void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			parentMapper.MapModifiedFlagsToMapFromEntity(session, data, newObj, oldObj);
			main.MapModifiedFlagsToMapFromEntity(session, data, newObj, oldObj);
		}

		public void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data)
		{
			parentMapper.MapModifiedFlagsToMapForCollectionChange(collectionPropertyName, data);
			main.MapModifiedFlagsToMapForCollectionChange(collectionPropertyName, data);
		}

		public ICompositeMapperBuilder AddComponent(PropertyData propertyData, string componentClassName) 
		{
			return main.AddComponent(propertyData, componentClassName);
		}

		public void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper) 
		{
			main.AddComposite(propertyData, propertyMapper);
		}

		public IDictionary<PropertyData, IPropertyMapper> Properties
		{
			get
			{
				var joinedProperties = new Dictionary<PropertyData, IPropertyMapper>();
				foreach (var propertyMapper in parentMapper.Properties)
				{
					joinedProperties[propertyMapper.Key] = propertyMapper.Value;
				}
				foreach (var propertyMapper in main.Properties)
				{
					joinedProperties[propertyMapper.Key] = propertyMapper.Value;
				}
				return joinedProperties;
			}
		}

		public void Add(PropertyData propertyData) 
		{
			main.Add(propertyData);
		}
	}
}

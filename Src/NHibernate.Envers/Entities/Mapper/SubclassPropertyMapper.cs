using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Collection;

namespace NHibernate.Envers.Entities.Mapper
{
	/**
	 * A mapper which maps from a parent mapper and a "main" one, but adds only to the "main". The "main" mapper
	 * should be the mapper of the subclass.
	 * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
	 */
	public class SubclassPropertyMapper : IExtendedPropertyMapper {
		private IExtendedPropertyMapper main;
		private IExtendedPropertyMapper parentMapper;

		public SubclassPropertyMapper(IExtendedPropertyMapper main, IExtendedPropertyMapper parentMapper) {
			this.main = main;
			this.parentMapper = parentMapper;
		}

		public bool Map(ISessionImplementor session, IDictionary<String, Object> data, String[] propertyNames, Object[] newState, Object[] oldState) {
			bool parentDiffs = parentMapper.Map(session, data, propertyNames, newState, oldState);
			bool mainDiffs = main.Map(session, data, propertyNames, newState, oldState);

			return parentDiffs || mainDiffs;
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<String, Object> data, Object newObj, Object oldObj) {
			bool parentDiffs = parentMapper.MapToMapFromEntity(session, data, newObj, oldObj);
			bool mainDiffs = main.MapToMapFromEntity(session, data, newObj, oldObj);

			return parentDiffs || mainDiffs;
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, Object obj, IDictionary<string,object> data, Object primaryKey, IAuditReaderImplementor versionsReader, long revision) {
			parentMapper.MapToEntityFromMap(verCfg, obj, data, primaryKey, versionsReader, revision);
			main.MapToEntityFromMap(verCfg, obj, data, primaryKey, versionsReader, revision);
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(String referencingPropertyName,
																						IPersistentCollection newColl, 
																						Object oldColl,
																						Object id) {
			IList<PersistentCollectionChangeData> parentCollectionChanges = parentMapper.MapCollectionChanges(
					referencingPropertyName, newColl, oldColl, id);

			IList<PersistentCollectionChangeData> mainCollectionChanges = main.MapCollectionChanges(
					referencingPropertyName, newColl, oldColl, id);

			if (parentCollectionChanges == null) {
				return mainCollectionChanges;
			} else {
				if(mainCollectionChanges != null) {
					parentCollectionChanges.Concat(mainCollectionChanges);
				}
				return parentCollectionChanges;
			}
		}

		public ICompositeMapperBuilder AddComponent(PropertyData propertyData, String componentClassName) {
			return main.AddComponent(propertyData, componentClassName);
		}

		public void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper) {
			main.AddComposite(propertyData, propertyMapper);
		}

		public void Add(PropertyData propertyData) {
			main.Add(propertyData);
		}
	}
}

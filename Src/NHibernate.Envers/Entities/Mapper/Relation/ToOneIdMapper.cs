using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Engine;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Collection;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class ToOneIdMapper : IPropertyMapper
	{
		private readonly IIdMapper _delegat;
		private readonly PropertyData _propertyData;
		private readonly string _referencedEntityName;
		private readonly bool _nonInsertableFake;

		public ToOneIdMapper(IIdMapper delegat, PropertyData propertyData, string referencedEntityName, bool nonInsertableFake)
		{
			_delegat = delegat;
			_propertyData = propertyData;
			_referencedEntityName = referencedEntityName;
			_nonInsertableFake = nonInsertableFake;
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			var newData = new Dictionary<string, object>();
			data[_propertyData.Name] = newData;

			// If this property is originally non-insertable, but made insertable because it is in a many-to-one "fake"
			// bi-directional relation, we always store the "old", unchaged data, to prevent storing changes made
			// to this field. It is the responsibility of the collection to properly update it if it really changed.
			_delegat.MapToMapFromEntity(newData, _nonInsertableFake ? oldObj : newObj);

			//noinspection SimplifiableConditionalExpression
			return _nonInsertableFake ? false : !Toolz.EntitiesEqual(session, newObj, oldObj);
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary<string, object> data, object primaryKey,
									   IAuditReaderImplementor versionsReader, long revision)
		{
			if (obj == null)
			{
				return;
			}

			var entityId = _delegat.MapToIdFromMap(DictionaryWrapper<string, object>.Wrap((IDictionary)data[_propertyData.Name]));
			object value;
			if (entityId == null)
			{
				value = null;
			}
			else
			{
				if (versionsReader.FirstLevelCache.Contains(_referencedEntityName, revision, entityId))
				{
					value = versionsReader.FirstLevelCache[_referencedEntityName, revision, entityId];
				}
				else
				{
					//java: Class<?> entityClass = ReflectionTools.loadClass(referencedEntityName); 
					value = versionsReader.SessionImplementor.Factory.GetEntityPersister(_referencedEntityName).CreateProxy(
						entityId, new ToOneDelegateSessionImplementor(versionsReader, Toolz.ResolveDotnetType(_referencedEntityName), entityId, revision, verCfg));
				}
			}
			var setter = ReflectionTools.GetSetter(obj.GetType(), _propertyData);
			setter.Set(obj, value);
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(string referencingPropertyName,
																		 IPersistentCollection newColl,
																		 object oldColl,
																		 object id)
		{
			return null;
		}
	}
}

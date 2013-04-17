using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public abstract class AbstractCollectionMapper : IPropertyMapper
	{
		private readonly IEnversProxyFactory _enversProxyFactory;
		private readonly System.Type _proxyType;
		private readonly bool _ordinalInId;
		private readonly bool _revisionTypeInId;

		protected AbstractCollectionMapper(IEnversProxyFactory enversProxyFactory, 
											CommonCollectionMapperData commonCollectionMapperData,
											System.Type proxyType, bool ordinalInId, bool revisionTypeInId) 
		{
			CommonCollectionMapperData = commonCollectionMapperData;
			_enversProxyFactory = enversProxyFactory;
			_proxyType = proxyType;
			_ordinalInId = ordinalInId;
			_revisionTypeInId = revisionTypeInId;
		}

		protected CommonCollectionMapperData CommonCollectionMapperData { get; private set; }

		protected abstract IEnumerable GetNewCollectionContent(IPersistentCollection newCollection);
		protected abstract IEnumerable GetOldCollectionContent(object oldCollection);

		/// <summary>
		/// Maps the changed collection element to the given map.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="idData">Map to which composite-id data should be added</param>
		/// <param name="data">Where to map the data.</param>
		/// <param name="changed">The changed collection element to map.</param>
		protected abstract void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object changed);

		/// <summary>
		/// Creates map for storing identifier data. Ordinal parameter guarantees uniqueness of primary key.
		/// Composite primary key cannot contain embeddable properties since they might be nullable.
		/// </summary>
		/// <param name="ordinal">Iteration ordinal.</param>
		/// <returns>Map for holding identifier data.</returns>
		protected IDictionary<string, object> CreateIdMap(int ordinal)
		{
			var idMap = new Dictionary<string, object>();
			if (_ordinalInId)
			{
				idMap.Add(CommonCollectionMapperData.VerEntCfg.EmbeddableSetOrdinalPropertyName, ordinal);
			}
			return idMap;
		}

		private void addCollectionChanges(ISessionImplementor session, ICollection<PersistentCollectionChangeData> collectionChanges, 
											IEnumerable changed, 
											RevisionType revisionType, 
											object id)
		{
			var ordinal = 0;
			foreach (var changedObj in changed) 
			{
				var entityData = new Dictionary<string, object>();
				var originalId = CreateIdMap(ordinal++);
				entityData.Add(CommonCollectionMapperData.VerEntCfg.OriginalIdPropName, originalId);

				collectionChanges.Add(new PersistentCollectionChangeData(
						CommonCollectionMapperData.VersionsMiddleEntityName, entityData, changedObj));
				// Mapping the collection owner's id.
				CommonCollectionMapperData.ReferencingIdData.PrefixedMapper.MapToMapFromId(originalId, id);

				// Mapping collection element and index (if present).
				MapToMapFromObject(session, originalId, entityData, changedObj);

				(_revisionTypeInId ? originalId : entityData).Add(CommonCollectionMapperData.VerEntCfg.RevisionTypePropName, revisionType);
			}
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(ISessionImplementor session, string referencingPropertyName,
																			IPersistentCollection newColl,
																			object oldColl, 
																			object id) 
		{
			if (!CommonCollectionMapperData.CollectionReferencingPropertyData.Name.Equals(referencingPropertyName)) 
			{
				return null;
			}

			var collectionChanges = new List<PersistentCollectionChangeData>();

			// Comparing new and old collection content.
			var newCollection = GetNewCollectionContent(newColl);
			var oldCollection = GetOldCollectionContent(oldColl);

			var added = new HashSet<object>();
			if (newColl != null)
			{
				foreach (var item in newCollection)
				{
					added.Add(item);
				}
			}
			// Re-hashing the old collection as the hash codes of the elements there may have changed, and the
			// removeAll in AbstractSet has an implementation that is hashcode-change sensitive (as opposed to addAll).
			if (oldColl != null)
			{
				var itemsToRemove = new HashSet<object>();
				foreach (var item in oldCollection)
				{
					itemsToRemove.Add(item);
				}
				added.ExceptWith(itemsToRemove);
			}

			addCollectionChanges(session, collectionChanges, added, RevisionType.Added, id);

			var deleted = new HashSet<object>();
			if (oldColl != null)
			{
				foreach (var item in oldCollection)
				{
					deleted.Add(item);
				}
			}
			// The same as above - re-hashing new collection.
			if (newColl != null)
			{
				var itemsToRemove = new HashSet<object>();
				foreach (var item in newCollection)
				{
					itemsToRemove.Add(item);
				}
				deleted.ExceptWith(itemsToRemove);
			}

			addCollectionChanges(session, collectionChanges, deleted, RevisionType.Deleted, id);

			return collectionChanges;
		}

		public void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			var propertyData = CommonCollectionMapperData.CollectionReferencingPropertyData;
			if (propertyData.UsingModifiedFlag)
			{
				if (isNotPersistentCollection(newObj) || isNotPersistentCollection(oldObj))
				{
					//Compare POCOs
					data[propertyData.ModifiedFlagPropertyName] = !Toolz.ObjectsEqual(newObj, oldObj);
				}
				else
				{
					var newObjAsPersistentCollection = (IPersistentCollection)newObj;
					if (isFromNullToEmptyOrFromEmptyToNull(newObjAsPersistentCollection, oldObj))
					{
						data[propertyData.ModifiedFlagPropertyName] = true;
					}
					else
					{
						var changes = MapCollectionChanges(session, propertyData.Name, newObjAsPersistentCollection, oldObj, null);
						data[propertyData.ModifiedFlagPropertyName] = changes.Any();
					}					
				}
			}
		}

		private static bool isNotPersistentCollection(object obj)
		{
			return obj != null && !(obj is IPersistentCollection);
		}

		private bool isFromNullToEmptyOrFromEmptyToNull(IPersistentCollection newColl, object oldColl)
		{
			// Comparing new and old collection content.
			var newCollection = GetNewCollectionContent(newColl);
			var oldCollection = GetOldCollectionContent(oldColl);
			return oldCollection == null && newCollection != null && isEmpty(newCollection)
			       || newCollection == null && oldCollection != null && isEmpty(oldCollection);
		}

		private static bool isEmpty(IEnumerable source)
		{
			return !source.Cast<object>().Any();
		}

		public void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data)
		{
			var propertyData = CommonCollectionMapperData.CollectionReferencingPropertyData;
			if (propertyData.UsingModifiedFlag)
			{
				data[propertyData.ModifiedFlagPropertyName] = propertyData.Name.Equals(collectionPropertyName);
			}
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj) 
		{
			// Changes are mapped in the "mapCollectionChanges" method.
			return false;
		}

		protected abstract IInitializor GetInitializor(AuditConfiguration verCfg,
														IAuditReaderImplementor versionsReader, 
														object primaryKey,
														long revision,
														bool removed);

		public void MapToEntityFromMap(AuditConfiguration verCfg, 
										object obj, 
										IDictionary data, 
										object primaryKey, 
										IAuditReaderImplementor versionsReader, 
										long revision) 
		{
			var setter = ReflectionTools.GetSetter(obj.GetType(), CommonCollectionMapperData.CollectionReferencingPropertyData);
			var coll = _enversProxyFactory.CreateCollectionProxy(_proxyType,
			                                                     GetInitializor(verCfg, versionsReader, primaryKey, revision,
			                                                                    data[verCfg.AuditEntCfg.RevisionTypePropName]
				                                                                    .Equals(RevisionType.Deleted)));
			setter.Set(obj, coll);
		}
	}
}

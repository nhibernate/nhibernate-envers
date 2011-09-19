using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using Iesi.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public abstract class AbstractCollectionMapper : IPropertyMapper
	{
		private readonly System.Type _proxyType;

		protected AbstractCollectionMapper(CommonCollectionMapperData commonCollectionMapperData,
											System.Type collectionType, 
											System.Type proxyType) 
		{
			CommonCollectionMapperData = commonCollectionMapperData;
			CollectionType = collectionType;
			_proxyType = proxyType;
		}

		protected CommonCollectionMapperData CommonCollectionMapperData { get; private set; }
		protected System.Type CollectionType { get; private set; }

		protected abstract IEnumerable GetNewCollectionContent(IPersistentCollection newCollection);
		protected abstract IEnumerable GetOldCollectionContent(object oldCollection);

		/// <summary>
		/// Maps the changed collection element to the given map.
		/// </summary>
		/// <param name="data">Where to map the data.</param>
		/// <param name="changed">The changed collection element to map.</param>
		protected abstract void MapToMapFromObject(IDictionary<string, object> data, object changed);

		private void addCollectionChanges(ICollection<PersistentCollectionChangeData> collectionChanges, 
											IEnumerable changed, 
											RevisionType revisionType, 
											object id) 
		{
			foreach (var changedObj in changed) 
			{
				var entityData = new Dictionary<string, object>();
				var originalId = new Dictionary<string, object>();
				entityData.Add(CommonCollectionMapperData.VerEntCfg.OriginalIdPropName, originalId);

				collectionChanges.Add(new PersistentCollectionChangeData(
						CommonCollectionMapperData.VersionsMiddleEntityName, entityData, changedObj));
				// Mapping the collection owner's id.
				CommonCollectionMapperData.ReferencingIdData.PrefixedMapper.MapToMapFromId(originalId, id);

				// Mapping collection element and index (if present).
				MapToMapFromObject(originalId, changedObj);

				entityData.Add(CommonCollectionMapperData.VerEntCfg.RevisionTypePropName, revisionType);
			}
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(string referencingPropertyName,
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

			var added = new HashedSet();
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
				var itemsToRemove = new HashedSet();
				foreach (var item in oldCollection)
				{
					itemsToRemove.Add(item);
				}
				added.RemoveAll(itemsToRemove);
			}

			addCollectionChanges(collectionChanges, added, RevisionType.Added, id);

			var deleted = new HashedSet();
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
				var itemsToRemove = new HashedSet();
				foreach (var item in newCollection)
				{
					itemsToRemove.Add(item);
				}
				deleted.RemoveAll(itemsToRemove);
			}

			addCollectionChanges(collectionChanges, deleted, RevisionType.Deleted, id);

			return collectionChanges;
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj) 
		{
			// Changes are mapped in the "mapCollectionChanges" method.
			return false;
		}

		protected abstract object GetInitializor(AuditConfiguration verCfg,
														IAuditReaderImplementor versionsReader, 
														object primaryKey,
														long revision);

		public void MapToEntityFromMap(AuditConfiguration verCfg, 
										object obj, 
										IDictionary data, 
										object primaryKey, 
										IAuditReaderImplementor versionsReader, 
										long revision) 
		{
			var setter = ReflectionTools.GetSetter(obj.GetType(),
												   CommonCollectionMapperData.CollectionReferencingPropertyData);

			try 
			{
				var coll = Activator.CreateInstance(_proxyType, new[]{GetInitializor(verCfg, versionsReader, primaryKey, revision)});
				setter.Set(obj, coll);
			} 
			catch (InstantiationException e) 
			{
				throw new AuditException(e.Message, e);
			} 
			catch (SecurityException e) 
			{
				throw new AuditException(e.Message, e);
			} 
			catch (TargetInvocationException e) 
			{
				throw new AuditException(e.Message, e);
			}
		}
	}
}

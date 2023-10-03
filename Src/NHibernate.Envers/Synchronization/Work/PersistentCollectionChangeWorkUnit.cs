using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Synchronization.Work
{
	public partial class PersistentCollectionChangeWorkUnit : AbstractAuditWorkUnit 
	{
		private readonly IList<PersistentCollectionChangeData> collectionChanges;
		private readonly string referencingPropertyName;

		public PersistentCollectionChangeWorkUnit(ISessionImplementor sessionImplementor, 
												string entityName,
												AuditConfiguration auditCfg, 
												IPersistentCollection collection,
												CollectionEntry collectionEntry, 
												object snapshot, 
												object id,
												string referencingPropertyName) 
			: base(sessionImplementor, entityName, auditCfg, 
					new PersistentCollectionChangeWorkUnitId(id, collectionEntry.Role), RevisionType.Modified)
		{
			this.referencingPropertyName = referencingPropertyName;

			collectionChanges = auditCfg.EntCfg[EntityName].PropertyMapper
					.MapCollectionChanges(sessionImplementor, referencingPropertyName, collection, snapshot, id);
		}

		private PersistentCollectionChangeWorkUnit(ISessionImplementor sessionImplementor, 
												string entityName,
												AuditConfiguration verCfg, 
												object id,
												IList<PersistentCollectionChangeData> collectionChanges,
												string referencingPropertyName) 
			:base(sessionImplementor, entityName, verCfg, id, RevisionType.Modified)
		{
			this.collectionChanges = collectionChanges;
			this.referencingPropertyName = referencingPropertyName;
		}

		public override bool ContainsWork()
		{
			return collectionChanges != null && collectionChanges.Count != 0;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			throw new NotSupportedException("Cannot generate data for a collection change work unit!");
		}

		public override void Perform(ISession session, object revisionData) 
		{
			var entitiesCfg = VerCfg.AuditEntCfg;

			foreach (var persistentCollectionChangeData in collectionChanges) 
			{
				// Setting the revision number
				((IDictionary<string, object>) persistentCollectionChangeData.Data[entitiesCfg.OriginalIdPropName])
						.Add(entitiesCfg.RevisionFieldName, revisionData);
				VerCfg.GlobalCfg.AuditStrategy.PerformCollectionChange(session, EntityName, referencingPropertyName, VerCfg, persistentCollectionChangeData, revisionData);
			}
		}

		public IEnumerable<PersistentCollectionChangeData> CollectionChanges => collectionChanges;

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Merge(ModWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Merge(DelWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
		{
			if (first is PersistentCollectionChangeWorkUnit original) 
			{
			
				// Merging the collection changes in both work units.

				// First building a map from the ids of the collection-entry-entities from the "second" collection changes,
				// to the PCCD objects. That way, we will be later able to check if an "original" collection change
				// should be added, or if it is overshadowed by a new one.
				var newChangesIdMap = new Dictionary<IDictionary<string, object>, PersistentCollectionChangeData>(new DictionaryComparer<string, object>());
				foreach (var persistentCollectionChangeData in CollectionChanges) 
				{
					newChangesIdMap.Add(
							originalId(persistentCollectionChangeData),
							persistentCollectionChangeData);
				}

				// This will be the list with the resulting (merged) changes.
				var mergedChanges = new List<PersistentCollectionChangeData>();

				// Including only those original changes, which are not overshadowed by new ones.
				foreach (var originalCollectionChangeData in original.CollectionChanges) 
				{
					var originalOriginalId = originalId(originalCollectionChangeData);

					if (!newChangesIdMap.ContainsKey(originalOriginalId)) 
					{
						mergedChanges.Add(originalCollectionChangeData);
					}
					else
					{
						// If the changes collide, checking if the first one isn't a DEL, and the second a subsequent ADD
						// If so, removing the change alltogether.
						var revTypePropName = VerCfg.AuditEntCfg.RevisionTypePropName;
						if((RevisionType)newChangesIdMap[originalOriginalId].Data[revTypePropName] == RevisionType.Added &&
							(RevisionType)originalCollectionChangeData.Data[revTypePropName] == RevisionType.Deleted)
						{
							newChangesIdMap.Remove(originalOriginalId);
						}
					}
				}

				// Finally adding all of the new changes to the end of the list
				// (the map values may differ from CollectionChanges because of the last operation above)
				mergedChanges = mergedChanges.Concat(newChangesIdMap.Values).ToList();

				return new PersistentCollectionChangeWorkUnit(SessionImplementor, EntityName, VerCfg, EntityId, mergedChanges, 
						referencingPropertyName);
			}
			throw new Exception("Trying to merge a " + first + " with a PersitentCollectionChangeWorkUnit. " +
								"This is not really possible.");
		}

		private IDictionary<string, object> originalId(PersistentCollectionChangeData persistentCollectionChangeData) 
		{
			return (IDictionary<string, object>) persistentCollectionChangeData.Data[VerCfg.AuditEntCfg.OriginalIdPropName];
		}

		/// <summary>
		/// A unique identifier for a collection work unit. Consists of an id of the owning entity and the name of
		/// the entity plus the name of the field (the role). This is needed because such collections aren't entities
		/// in the "normal" mapping, but they are entities for Envers.
		/// </summary>
		[Serializable]
		public class PersistentCollectionChangeWorkUnitId
		{
			private readonly string role;

			public PersistentCollectionChangeWorkUnitId(object ownerId, string role) 
			{
				OwnerId = ownerId;
				this.role = role;
			}

			public object OwnerId { get; }
			
			public override bool Equals(object o) 
			{
				if (this == o) return true;
				if (o == null || GetType() != o.GetType()) return false;

				var that = (PersistentCollectionChangeWorkUnitId) o;

				if (!OwnerId?.Equals(that.OwnerId) ?? that.OwnerId != null) return false;
				if (!role?.Equals(that.role) ?? that.role != null) return false;

				return true;
			}

			public override int GetHashCode() 
			{
				var result = OwnerId?.GetHashCode() ?? 0;
				result = 31 * result + (role?.GetHashCode() ?? 0);
				return result;
			}
		}
	}
}

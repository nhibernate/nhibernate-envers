using System;
using NHibernate.Event;
using NHibernate.Proxy;
using NHibernate.Engine;
using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Synchronization.Work;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Synchronization;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Event
{
	public class AuditEventListener: IPostInsertEventListener, IPostUpdateEventListener,
			IPostDeleteEventListener, IPreCollectionUpdateEventListener, IPreCollectionRemoveEventListener,
			IPostCollectionRecreateEventListener, IInitializable 
	{
		private AuditConfiguration verCfg;

		private void GenerateBidirectionalCollectionChangeWorkUnits(AuditSync verSync, IEntityPersister entityPersister,
																	string entityName, object[] newState, object[] oldState,
																	ISessionImplementor session) {
			// Checking if this is enabled in configuration ...
			if (!verCfg.GlobalCfg.GenerateRevisionsForCollections) 
			{
				return;
			}

			// Checks every property of the entity, if it is an "owned" to-one relation to another entity.
			// If the value of that property changed, and the relation is bi-directional, a new revision
			// for the related entity is generated.
			var propertyNames = entityPersister.PropertyNames;

			for (var i=0; i<propertyNames.GetLength(0); i++) 
			{
				var propertyName = propertyNames[i];
				var relDesc = verCfg.EntCfg.GetRelationDescription(entityName, propertyName);
				if (relDesc != null && relDesc.Bidirectional && relDesc.RelationType == RelationType.TO_ONE &&
						relDesc.Insertable) 
				{
					// Checking for changes
					var oldValue = oldState == null ? null : oldState[i];
					var newValue = newState == null ? null : newState[i];

					if (!Toolz.EntitiesEqual(session, oldValue, newValue)) 
					{
						// We have to generate changes both in the old collection (size decreses) and new collection
						// (size increases).

						//<TODO Simon: doua if-uri cu cod duplicat, refact.
						if (newValue != null) {
							// relDesc.getToEntityName() doesn't always return the entity name of the value - in case
							// of subclasses, this will be root class, no the actual class. So it can't be used here.
							string toEntityName;
							
							// Java: Serializable id
							object id;

							var newValueAsProxy = newValue as INHibernateProxy;
							if (newValueAsProxy != null) 
							{
								toEntityName = session.BestGuessEntityName(newValue);
								id = newValueAsProxy.HibernateLazyInitializer.Identifier;
								// We've got to initialize the object from the proxy to later read its state.   
								newValue = Toolz.GetTargetFromProxy(session.Factory, newValueAsProxy);
							} 
							else 
							{
								toEntityName =  session.GuessEntityName(newValue);

								var idMapper = verCfg.EntCfg[toEntityName].GetIdMapper();
								id = idMapper.MapToIdFromEntity(newValue);
							}

							verSync.AddWorkUnit(new CollectionChangeWorkUnit(session, toEntityName, verCfg, id, newValue));
						}

						if (oldValue != null) 
						{
							string toEntityName;
							object id;

							var oldValueAsProxy = oldValue as INHibernateProxy;
							if (oldValueAsProxy != null) 
							{
								toEntityName = session.BestGuessEntityName(oldValue);
								id = oldValueAsProxy.HibernateLazyInitializer.Identifier;
								// We've got to initialize the object as we'll read it's state anyway.
								oldValue = Toolz.GetTargetFromProxy(session.Factory, oldValueAsProxy);
							} 
							else 
							{
								toEntityName =  session.GuessEntityName(oldValue);

								var idMapper = verCfg.EntCfg[toEntityName].GetIdMapper();
								id = idMapper.MapToIdFromEntity(oldValue);
							}
							
							verSync.AddWorkUnit(new CollectionChangeWorkUnit(session, toEntityName, verCfg, id, oldValue));
						}
					}
				}
			}
		}

		public virtual void OnPostInsert(PostInsertEvent evt) 
		{
			var entityName = evt.Persister.EntityName;
			if (verCfg.EntCfg.IsVersioned(entityName)) 
			{
				var verSync = verCfg.AuditSyncManager.get(evt.Session);

				var workUnit = new AddWorkUnit(evt.Session, evt.Persister.EntityName, verCfg,
						evt.Id, evt.Persister, evt.State);
				verSync.AddWorkUnit(workUnit);

				if (workUnit.ContainsWork()) 
				{
					GenerateBidirectionalCollectionChangeWorkUnits(verSync, evt.Persister, entityName, evt.State,
							null, evt.Session);
				}

				//Simon - TODO - Correct/clarify this:
				// it appears that the AuditSyncManager's transaction.RegisterSynchronization(verSync);
				// does not lead to calling the verSync's synchronization methods (Before and AfterCompletion
				// so I will call this manually. The problem that I found is that AdoTransaction's Commit method
				// is not called at all. Could this be because of Spring.NET?
				// When corrected, change also in AuditSync the Flush in BeforeCompletion.
				//verSync.BeforeCompletion();
			}
		}

		public virtual void OnPostUpdate(PostUpdateEvent evt) 
		{
			var entityName = evt.Persister.EntityName;

			if (verCfg.EntCfg.IsVersioned(entityName)) 
			{
				var verSync = verCfg.AuditSyncManager.get(evt.Session);

				var workUnit = new ModWorkUnit(evt.Session, evt.Persister.EntityName, verCfg,
						evt.Id, evt.Persister, evt.State, evt.OldState);
				verSync.AddWorkUnit(workUnit);

				if (workUnit.ContainsWork()) 
				{
					GenerateBidirectionalCollectionChangeWorkUnits(verSync, evt.Persister, entityName, evt.State,
							evt.OldState, evt.Session);
				}
				//Simon - TODO - same as above
				//verSync.BeforeCompletion();
			}
		}

		public virtual void OnPostDelete(PostDeleteEvent evt) 
		{
			var entityName = evt.Persister.EntityName;

			if (verCfg.EntCfg.IsVersioned(entityName)) 
			{
				var verSync = verCfg.AuditSyncManager.get(evt.Session);

				var workUnit = new DelWorkUnit(evt.Session, evt.Persister.EntityName, verCfg,
						evt.Id, evt.Persister, evt.DeletedState);
				verSync.AddWorkUnit(workUnit);

				if (workUnit.ContainsWork()) 
				{
					GenerateBidirectionalCollectionChangeWorkUnits(verSync, evt.Persister, entityName, null,
							evt.DeletedState, evt.Session);
				}
			}
		}

		private void GenerateBidirectionalCollectionChangeWorkUnits(AuditSync verSync, AbstractCollectionEvent evt,
																	PersistentCollectionChangeWorkUnit workUnit,
																	RelationDescription rd) 
		{
			// Checking if this is enabled in configuration ...
			if (!verCfg.GlobalCfg.GenerateRevisionsForCollections) 
			{
				return;
			}

			// Checking if this is not a bidirectional relation - then, a revision needs also be generated for
			// the other side of the relation.
			// relDesc can be null if this is a collection of simple values (not a relation).
			if (rd != null && rd.Bidirectional) 
			{
				var relatedEntityName = rd.ToEntityName;
				var relatedIdMapper = verCfg.EntCfg[relatedEntityName].GetIdMapper();
				
				foreach (var changeData in workUnit.CollectionChanges) 
				{
					var relatedObj = changeData.GetChangedElement();
					var relatedId = relatedIdMapper.MapToIdFromEntity(relatedObj);

					verSync.AddWorkUnit(new CollectionChangeWorkUnit(evt.Session, relatedEntityName, verCfg,
							relatedId, relatedObj));
				}
			}
		}

		private void GenerateFakeBidirecationalRelationWorkUnits(AuditSync verSync, IPersistentCollection newColl, object oldColl,
																 String collectionEntityName, String referencingPropertyName,
																 AbstractCollectionEvent evt,
																 RelationDescription rd) {
			// First computing the relation changes
			var collectionChanges = verCfg.EntCfg[collectionEntityName].PropertyMapper
					.MapCollectionChanges(referencingPropertyName, newColl, oldColl, evt.AffectedOwnerIdOrNull);

			// Getting the id mapper for the related entity, as the work units generated will corrspond to the related
			// entities.
			var relatedEntityName = rd.ToEntityName;
			var relatedIdMapper = verCfg.EntCfg[relatedEntityName].GetIdMapper();

			// For each collection change, generating the bidirectional work unit.
			foreach (var changeData in collectionChanges) 
			{
				var relatedObj = changeData.GetChangedElement();
				var relatedId = relatedIdMapper.MapToIdFromEntity(relatedObj);
				var revType = (RevisionType)changeData.Data[verCfg.AuditEntCfg.RevisionTypePropName];

				// This can be different from relatedEntityName, in case of inheritance (the real entity may be a subclass
				// of relatedEntityName).
				var realRelatedEntityName = evt.Session.BestGuessEntityName(relatedObj);

				// By default, the nested work unit is a collection change work unit.
				var nestedWorkUnit = new CollectionChangeWorkUnit(evt.Session, realRelatedEntityName, verCfg,
						relatedId, relatedObj);

				verSync.AddWorkUnit(new FakeBidirectionalRelationWorkUnit(evt.Session, realRelatedEntityName, verCfg,
						relatedId, referencingPropertyName, evt.AffectedOwnerOrNull, rd, revType,
						changeData.GetChangedElementIndex(), nestedWorkUnit));
			}

			// We also have to generate a collection change work unit for the owning entity.
			verSync.AddWorkUnit(new CollectionChangeWorkUnit(evt.Session, collectionEntityName, verCfg,
					evt.AffectedOwnerIdOrNull, evt.AffectedOwnerOrNull));
		}

		private void OnCollectionAction(AbstractCollectionEvent evt, IPersistentCollection newColl, object oldColl,
										CollectionEntry collectionEntry) 
		{
			var entityName = evt.GetAffectedOwnerEntityName();

			if (verCfg.EntCfg.IsVersioned(entityName)) 
			{
				var verSync = verCfg.AuditSyncManager.get(evt.Session);

				var ownerEntityName = ((AbstractCollectionPersister) collectionEntry.LoadedPersister).OwnerEntityName;
				var referencingPropertyName = collectionEntry.Role.Substring(ownerEntityName.Length + 1);

				// Checking if this is not a "fake" many-to-one bidirectional relation. The relation description may be
				// null in case of collections of non-entities.
				var rd = verCfg.EntCfg[entityName].GetRelationDescription(referencingPropertyName);
				if (rd != null && rd.MappedByPropertyName != null) 
				{
					GenerateFakeBidirecationalRelationWorkUnits(verSync, newColl, oldColl, entityName,
							referencingPropertyName, evt, rd);
				} 
				else 
				{
					var workUnit = new PersistentCollectionChangeWorkUnit(evt.Session,
							entityName, verCfg, newColl, collectionEntry, oldColl, evt.AffectedOwnerIdOrNull,
							referencingPropertyName);
					verSync.AddWorkUnit(workUnit);

					if (workUnit.ContainsWork()) 
					{
						// There are some changes: a revision needs also be generated for the collection owner
						verSync.AddWorkUnit(new CollectionChangeWorkUnit(evt.Session, evt.GetAffectedOwnerEntityName(),
								verCfg, evt.AffectedOwnerIdOrNull, evt.AffectedOwnerOrNull));

						GenerateBidirectionalCollectionChangeWorkUnits(verSync, evt, workUnit, rd);
					}
				}
			}
		}

		private static CollectionEntry GetCollectionEntry(AbstractCollectionEvent evt) 
		{
			return evt.Session.PersistenceContext.GetCollectionEntry(evt.Collection);
		}

		public virtual void OnPreUpdateCollection(PreCollectionUpdateEvent evt) 
		{
			var collectionEntry = GetCollectionEntry(evt);
			if (!collectionEntry.LoadedPersister.IsInverse) 
			{
				OnCollectionAction(evt, evt.Collection, collectionEntry.Snapshot, collectionEntry);
			}
		}

		public virtual void OnPreRemoveCollection(PreCollectionRemoveEvent evt) 
		{
			var collectionEntry = GetCollectionEntry(evt);
			if (collectionEntry != null && !collectionEntry.LoadedPersister.IsInverse) 
			{
				OnCollectionAction(evt, null, collectionEntry.Snapshot, collectionEntry);
			}
		}

		public virtual void OnPostRecreateCollection(PostCollectionRecreateEvent evt) 
		{
			var collectionEntry = GetCollectionEntry(evt);
			if (!collectionEntry.LoadedPersister.IsInverse) 
			{
				OnCollectionAction(evt, evt.Collection, null, collectionEntry);
			}
		}

		public virtual void Initialize(Cfg.Configuration cfg) 
		{
			verCfg = AuditConfiguration.GetFor(cfg);
		}

		public AuditConfiguration GetVerCfg() 
		{
			return verCfg;
		}
	}
}

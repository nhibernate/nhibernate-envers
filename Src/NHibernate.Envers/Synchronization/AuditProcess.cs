using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Synchronization.Work;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Synchronization
{
	public partial class AuditProcess
	{
		private readonly IRevisionInfoGenerator revisionInfoGenerator;
		private readonly ISessionImplementor session;
		private readonly LinkedList<IAuditWorkUnit> workUnits;
		private readonly Queue<IAuditWorkUnit> undoQueue;
		private readonly IDictionary<Tuple<string, object>, IAuditWorkUnit> usedIds;
		private readonly EntityChangeNotifier entityChangeNotifier;
		private object revisionData;
		private bool revisionInfoPersistedInCurrentTransaction;

		public AuditProcess(IRevisionInfoGenerator revisionInfoGenerator, ISessionImplementor session)
		{
			this.revisionInfoGenerator = revisionInfoGenerator;
			this.session = session;

			workUnits = new LinkedList<IAuditWorkUnit>();
			undoQueue = new Queue<IAuditWorkUnit>();
			usedIds = new Dictionary<Tuple<string, object>, IAuditWorkUnit>();
			entityChangeNotifier = new EntityChangeNotifier(revisionInfoGenerator, session);
		}

		public void AddWorkUnit(IAuditWorkUnit vwu)
		{
			if (!vwu.ContainsWork()) return;

			var entityId = vwu.EntityId;
			if (entityId == null)
			{
				// Just adding the work unit - it's not associated with any persistent entity.
				//ORIG: workUnits.offer(vwu);
				workUnits.AddLast(vwu);
			}
			else
			{
				var entityName = vwu.EntityName;
				var usedIdsKey = new Tuple<string, object>(entityName, entityId);

				var other = alreadyScheduledWorkUnit(usedIdsKey);
				if(other!=null)
				{
					var result = vwu.Dispatch(other);

					if (result != other)
					{
						removeWorkUnit(other);

						if (result != null)
						{
							usedIds[usedIdsKey] = result;
							workUnits.AddLast(result);
						} // else: a null result means that no work unit should be kept
					} // else: the result is the same as the work unit already added. No need to do anything.
				}
				else
				{
					usedIds[usedIdsKey] = vwu;
					workUnits.AddLast(vwu);
				}
			}
		}

		/// <summary>
		/// Checks if another work unit associated with the same entity hierarchy and identifier has already been scheduled.
		/// </summary>
		/// <param name="idKey"> Work unit's identifier.</param>
		/// <returns>Corresponding work unit or <code>null</code> if no satisfying result was found.</returns>
		private IAuditWorkUnit alreadyScheduledWorkUnit(Tuple<string, object> idKey)
		{
			var entityMetamodel = session.Factory.GetEntityPersister(idKey.Item1).EntityMetamodel;
			var rootEntityName = entityMetamodel.RootName;
			var rootEntityMetamodel = session.Factory.GetEntityPersister(rootEntityName).EntityMetamodel;

			// Checking all possible subtypes, supertypes and the actual class.
			foreach (var entityName in rootEntityMetamodel.SubclassEntityNames)
			{
				var key = new Tuple<string, object>(entityName, idKey.Item2);
				if (usedIds.ContainsKey(key))
				{
					return usedIds[key];
				}
			}

			return null;
		}

		private void removeWorkUnit(IAuditWorkUnit vwu)
		{
			workUnits.Remove(vwu);
			if (vwu.IsPerformed())
			{
				// If this work unit has already been performed, it must be deleted (undone) first.
				undoQueue.Enqueue(vwu);
			}
		}

		private void executeInSession(ISession executeSession)
		{
			// Making sure the revision data is persisted.
			var currentRevisionData = CurrentRevisionData(executeSession, true);

			// First undoing any performed work units
			while (undoQueue.Count > 0)
			{
				var vwu = undoQueue.Dequeue();
				vwu.Undo(executeSession);
			}

			while (workUnits.Count > 0)
			{
				var vwu = workUnits.First.Value;
				workUnits.RemoveFirst();
				vwu.Perform(executeSession, revisionData);
				entityChangeNotifier.EntityChanged(currentRevisionData, vwu);
			}
		}


		public object CurrentRevisionData(ISession executeSession, bool persist)
		{
			// Generating the revision data if not yet generated
			if (revisionData == null)
			{
				revisionData = revisionInfoGenerator.Generate();
			}

			// Saving the revision data, if not yet saved and persist is true
			if (!revisionInfoPersistedInCurrentTransaction && persist)
			{
				revisionInfoGenerator.SaveRevisionData(executeSession, revisionData);
				revisionInfoPersistedInCurrentTransaction = true;
			}

			return revisionData;
		}

		public void DoBeforeTransactionCompletion()
		{
			if (workUnits.Count == 0 && undoQueue.Count == 0)
			{
				return;
			}
			var castedSession = (ISession)session;

			if (castedSession.FlushMode == FlushMode.Manual)
			{
				using (var tempSession = Toolz.CreateChildSession(castedSession))
				{
					executeInSession(tempSession);
					tempSession.Flush();
				}
			}
			else
			{
				executeInSession(castedSession);
				// Explicity flushing the session, as the auto-flush may have already happened.
				castedSession.Flush();
			}
		}
	}
}
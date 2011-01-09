using System;
using System.Collections.Generic;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Synchronization.Work;
using NHibernate.Transaction;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Event;

namespace NHibernate.Envers.Synchronization
{
	public class AuditSync : ISynchronization 
	{
		private readonly IRevisionInfoGenerator revisionInfoGenerator;
		private readonly AuditSyncManager manager;
		private readonly IEventSource session;

		private readonly ITransaction transaction;
		private readonly LinkedList<IAuditWorkUnit> workUnits;
		private readonly Queue<IAuditWorkUnit> undoQueue;
		private readonly IDictionary<Pair<String, Object>, IAuditWorkUnit> usedIds;

		private Object revisionData;

		public AuditSync(AuditSyncManager manager, IEventSource session, IRevisionInfoGenerator revisionInfoGenerator) 
		{
			this.manager = manager;
			this.session = session;
			this.revisionInfoGenerator = revisionInfoGenerator;

			transaction = session.Transaction;
			workUnits = new LinkedList<IAuditWorkUnit>();
			undoQueue = new Queue<IAuditWorkUnit>();
			usedIds = new Dictionary<Pair<String, Object>, IAuditWorkUnit>();
		}

		private void RemoveWorkUnit(IAuditWorkUnit vwu) 
		{
			workUnits.Remove(vwu);
			if (vwu.IsPerformed()) 
			{
				// If this work unit has already been performed, it must be deleted (undone) first.
				//TODO Simon offer is more user friendly than add - see if C# Enqueue is compatible (also below)
				//ORIG: undoQueue.offer(vwu);
				undoQueue.Enqueue(vwu);
			}
		}

		public void AddWorkUnit(IAuditWorkUnit vwu) 
		{
			if (vwu.ContainsWork()) 
			{
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
					var usedIdsKey = Pair<String, Object>.Make(entityName, entityId);

					if (usedIds.ContainsKey(usedIdsKey)) 
					{
						var other = usedIds[usedIdsKey];
						var result = vwu.Dispatch(other);

						if (result != other) 
						{
							RemoveWorkUnit(other);

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
		}

		private void ExecuteInSession(ISession session) 
		{
			// Making sure the revision data is persisted.
			GetCurrentRevisionData(session, true);

			IAuditWorkUnit vwu;

			// First undoing any performed work units
			while ( undoQueue.Count > 0)
			{
				vwu = undoQueue.Dequeue();
				vwu.Undo(session);
			}

			//ORIG: while ((vwu = workUnits.poll()) != null) {
			//    vwu.Perform(session, revisionData);
			//}
			while (workUnits.Count > 0) 
			{
				vwu = workUnits.First.Value;
				workUnits.RemoveFirst();
				vwu.Perform(session, revisionData);
			}
		}

		public Object GetCurrentRevisionData(ISession session, bool persist) 
		{
			// Generating the revision data if not yet generated
			if (revisionData == null) 
			{
				revisionData = revisionInfoGenerator.generate();
			}

			// Saving the revision data, if not yet saved and persist is true
			if (!session.Contains(revisionData) && persist) {
				revisionInfoGenerator.saveRevisionData(session, revisionData);
			}

			return revisionData;
		}

		public void BeforeCompletion() 
		{
			if (workUnits.Count == 0 && undoQueue.Count == 0) 
			{
				return;
			}

			try 
			{
				// see: http://www.jboss.com/index.html?module=bb&op=viewtopic&p=4178431
				//Simon: According to http://nhforge.org/blogs/nhibernate/archive/2008/12/21/identity-the-never-ending-story.aspx
				// Never si the equiv. of Hibernate's MANUAL 
				//if (FlushMode.IsManualFlushMode(session.getFlushMode()) || session.isClosed()) {
				if ((((ISession)session).FlushMode == FlushMode.Never) || session.IsClosed)
				{
					ISession temporarySession = null;
					try 
					{
						temporarySession = session.Factory.OpenSession(null, false,false, ConnectionReleaseMode.AfterStatement);

						ExecuteInSession(temporarySession);

						temporarySession.Flush();
					} 
					finally 
					{
						if (temporarySession != null) 
						{
							temporarySession.Close();
						}
					}
				} 
				else 
				{
					ExecuteInSession(session);

					// Explicity flushing the session, as the auto-flush may have already happened.
					// TODO Simon: Will have to re-enable this after I figure out how to call BeforeCompletion the 
					// proper way, see also AuditEventListener.
					((ISession)session).Flush();
				}
			} 
			catch (Exception) 
			{
				// Rolling back the transaction in case of any exceptions
				if (session.Transaction.IsActive) 
				{
					session.Transaction.Rollback();
				}
				throw;
			}
		}

		public void AfterCompletion(bool b) 
		{
			manager.Remove(transaction);
		}
	}
}

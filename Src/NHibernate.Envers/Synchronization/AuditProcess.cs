using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Synchronization.Work;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Synchronization
{
	public class AuditProcess
	{
		private readonly IRevisionInfoGenerator revisionInfoGenerator;
		private readonly ISessionImplementor session;

		private LinkedList<IAuditWorkUnit> workUnits;
		private Queue<IAuditWorkUnit> undoQueue;
		private IDictionary<Pair<string, object>, IAuditWorkUnit> usedIds;

		private object revisionData;

		public AuditProcess(IRevisionInfoGenerator revisionInfoGenerator, ISessionImplementor session)
		{
			this.revisionInfoGenerator = revisionInfoGenerator;
			this.session = session;

			workUnits = new LinkedList<IAuditWorkUnit>();
			undoQueue = new Queue<IAuditWorkUnit>();
			usedIds = new Dictionary<Pair<string, object>, IAuditWorkUnit>();
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
					var usedIdsKey = Pair<string, object>.Make(entityName, entityId);

					if (usedIds.ContainsKey(usedIdsKey))
					{
						var other = usedIds[usedIdsKey];
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

		private void executeInSession(ISession session)
		{
			// Making sure the revision data is persisted.
			CurrentRevisionData(session, true);

			IAuditWorkUnit vwu;

			// First undoing any performed work units
			while (undoQueue.Count > 0)
			{
				vwu = undoQueue.Dequeue();
				vwu.Undo(session);
			}

			while (workUnits.Count > 0)
			{
				vwu = workUnits.First.Value;
				workUnits.RemoveFirst();
				vwu.Perform(session, revisionData);
			}
		}

		public object CurrentRevisionData(ISession session, bool persist)
		{
			// Generating the revision data if not yet generated
			if (revisionData == null)
			{
				revisionData = revisionInfoGenerator.Generate();
			}

			// Saving the revision data, if not yet saved and persist is true
			if (!session.Contains(revisionData) && persist)
			{
				revisionInfoGenerator.SaveRevisionData(session, revisionData);
			}

			return revisionData;
		}

		public void doBeforeTransactionCompletion()
		{
			if (workUnits.Count == 0 && undoQueue.Count == 0)
			{
				return;
			}

			if (FlushMode.Never == session.FlushMode)
			{

				using (var tempSession = session.Factory.OpenSession())
				{
					executeInSession(tempSession);
					tempSession.Flush();
				}
			}
			else
			{
				executeInSession((ISession)session);
				// Explicity flushing the session, as the auto-flush may have already happened.
				session.Flush();
			}
		}
	}
}
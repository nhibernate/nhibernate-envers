using System.Collections.Generic;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Event;

namespace NHibernate.Envers.Synchronization
{
	public class AuditSyncManager 
	{
		private readonly IDictionary<ITransaction, AuditSync> auditSyncs;
		private readonly IRevisionInfoGenerator revisionInfoGenerator;

		public AuditSyncManager(IRevisionInfoGenerator revisionInfoGenerator) 
		{
			//ORIG: auditSyncs = new ConcurrentHashMap<ITransaction, AuditSync>(); TODO Simon see if it's OK
			auditSyncs = new Dictionary<ITransaction, AuditSync>();

			this.revisionInfoGenerator = revisionInfoGenerator;
		}

		public AuditSync get(IEventSource session) 
		{
			var transaction = session.Transaction;

			if( auditSyncs.Keys.Contains(transaction))
				return auditSyncs[transaction];
			// No worries about registering a transaction twice - a transaction is single thread
			var verSync = new AuditSync(this, session, revisionInfoGenerator);
			auditSyncs.Add(transaction, verSync);

			transaction.RegisterSynchronization(verSync);

			//ITransactionSynchronization synchro = new EnversTransactionSynchronization(verSync);
			//TransactionSynchronizationManager.RegisterSynchronization(synchro);

			return verSync;
		}

		public void Remove(ITransaction transaction) 
		{
			auditSyncs.Remove(transaction);
		}
	}
}

using System.Collections.Generic;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Event;
using NHibernate.Util;

namespace NHibernate.Envers.Synchronization
{
	public class AuditSyncManager 
	{
		private readonly IDictionary<ITransaction, AuditSync> auditSyncs;
		private readonly IRevisionInfoGenerator revisionInfoGenerator;

		public AuditSyncManager(IRevisionInfoGenerator revisionInfoGenerator) 
		{
			auditSyncs = new ThreadSafeDictionary<ITransaction, AuditSync>(new Dictionary<ITransaction, AuditSync>());

			this.revisionInfoGenerator = revisionInfoGenerator;
		}

		public AuditSync Get(IEventSource session) 
		{
			var transaction = session.Transaction;
			AuditSync verSync;
			if(auditSyncs.TryGetValue(transaction, out verSync))
			{
				return verSync;
			}

			// No worries about registering a transaction twice - a transaction is single thread
			verSync = new AuditSync(this, session, revisionInfoGenerator);
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

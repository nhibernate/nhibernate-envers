using System;
using System.Collections.Generic;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Event;
using NHibernate.Util;

namespace NHibernate.Envers.Synchronization
{
	[Serializable]
	public class AuditProcessManager
	{
		private readonly IDictionary<ITransaction, AuditProcess> auditProcesses;
		private readonly IRevisionInfoGenerator revisionInfoGenerator;

		public AuditProcessManager(IRevisionInfoGenerator revisionInfoGenerator)
		{
			auditProcesses = new ThreadSafeDictionary<ITransaction, AuditProcess>(new Dictionary<ITransaction, AuditProcess>());
			this.revisionInfoGenerator = revisionInfoGenerator;
		}

		public AuditProcess Get(IEventSource session)
		{
			var transaction = session.Transaction;

			AuditProcess auditProcess;
			if (!auditProcesses.TryGetValue(transaction, out auditProcess))
			{
				// No worries about registering a transaction twice - a transaction is single thread
				auditProcess = new AuditProcess(revisionInfoGenerator, session);
				auditProcesses[transaction] = auditProcess;

				session.ActionQueue.RegisterProcess(auditProcess.DoBeforeTransactionCompletion);
				session.ActionQueue.RegisterProcess(success => auditProcesses.Remove(transaction));
			}

			return auditProcess;
		}
	}
}
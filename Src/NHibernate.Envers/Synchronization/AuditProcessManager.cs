using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Event;

namespace NHibernate.Envers.Synchronization
{
	[Serializable]
	public class AuditProcessManager
	{
		private readonly IDictionary<ITransaction, AuditProcess> _auditProcesses;
		private readonly IRevisionInfoGenerator _revisionInfoGenerator;

		public AuditProcessManager(IRevisionInfoGenerator revisionInfoGenerator)
		{
			_auditProcesses = new ConcurrentDictionary<ITransaction, AuditProcess>();
			_revisionInfoGenerator = revisionInfoGenerator;
		}

		public AuditProcess Get(IEventSource session)
		{
			var transaction = session.Transaction;

			AuditProcess auditProcess;
			if (!_auditProcesses.TryGetValue(transaction, out auditProcess))
			{
				// No worries about registering a transaction twice - a transaction is single thread
				auditProcess = new AuditProcess(_revisionInfoGenerator, session);
				_auditProcesses[transaction] = auditProcess;

				session.ActionQueue.RegisterProcess(() =>
				                                    	{
				                                    		AuditProcess currentProcess;
																		if(_auditProcesses.TryGetValue(transaction, out currentProcess))
																		{
																			currentProcess.DoBeforeTransactionCompletion();
																		}
				                                    	});
				session.ActionQueue.RegisterProcess(success => _auditProcesses.Remove(transaction));
			}

			return auditProcess;
		}
	}
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Action;
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

			if (!_auditProcesses.TryGetValue(transaction, out var auditProcess))
			{
				// No worries about registering a transaction twice - a transaction is single thread
				auditProcess = new AuditProcess(_revisionInfoGenerator, session);
				_auditProcesses[transaction] = auditProcess;
				var tranProcess = new transactionCompletionProcess(_auditProcesses, transaction);
				session.ActionQueue.RegisterProcess((IBeforeTransactionCompletionProcess)tranProcess);
				session.ActionQueue.RegisterProcess((IAfterTransactionCompletionProcess)tranProcess);
			}

			return auditProcess;
		}

		private class transactionCompletionProcess : IBeforeTransactionCompletionProcess, IAfterTransactionCompletionProcess
		{
			private readonly IDictionary<ITransaction, AuditProcess> _auditProcesses;
			private readonly ITransaction _transaction;

			public transactionCompletionProcess(IDictionary<ITransaction, AuditProcess> auditProcesses, ITransaction transaction)
			{
				_auditProcesses = auditProcesses;
				_transaction = transaction;
			}
			
			public void ExecuteBeforeTransactionCompletion()
			{
				if(_auditProcesses.TryGetValue(_transaction, out var currentProcess))
				{
					currentProcess.DoBeforeTransactionCompletion();
				}
			}
			
			public void ExecuteAfterTransactionCompletion(bool success)
			{
				_auditProcesses.Remove(_transaction);
			}

			public Task ExecuteBeforeTransactionCompletionAsync(CancellationToken cancellationToken)
			{
				ExecuteBeforeTransactionCompletion();
				return Task.CompletedTask;
			}

			public Task ExecuteAfterTransactionCompletionAsync(bool success, CancellationToken cancellationToken)
			{
				ExecuteAfterTransactionCompletion(success);
				return Task.CompletedTask;
			}
		}
	}
}
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Synchronization.Work;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Synchronization
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class AuditProcess
	{

		private async Task executeInSessionAsync(ISession executeSession, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			// Making sure the revision data is persisted.
			var currentRevisionData = await (CurrentRevisionDataAsync(executeSession, true, cancellationToken)).ConfigureAwait(false);

			// First undoing any performed work units
			while (undoQueue.Count > 0)
			{
				var vwu = undoQueue.Dequeue();
				await (vwu.UndoAsync(executeSession, cancellationToken)).ConfigureAwait(false);
			}

			while (workUnits.Count > 0)
			{
				var vwu = workUnits.First.Value;
				workUnits.RemoveFirst();
				await (vwu.PerformAsync(executeSession, revisionData, cancellationToken)).ConfigureAwait(false);
				entityChangeNotifier.EntityChanged(currentRevisionData, vwu);
			}
		}


		public async Task<object> CurrentRevisionDataAsync(ISession executeSession, bool persist, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			// Generating the revision data if not yet generated
			if (revisionData == null)
			{
				revisionData = revisionInfoGenerator.Generate();
			}

			// Saving the revision data, if not yet saved and persist is true
			if (!revisionInfoPersistedInCurrentTransaction && persist)
			{
				await (revisionInfoGenerator.SaveRevisionDataAsync(executeSession, revisionData, cancellationToken)).ConfigureAwait(false);
				revisionInfoPersistedInCurrentTransaction = true;
			}

			return revisionData;
		}

		public async Task DoBeforeTransactionCompletionAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (workUnits.Count == 0 && undoQueue.Count == 0)
			{
				return;
			}
			var castedSession = (ISession)session;

			if (castedSession.FlushMode == FlushMode.Manual)
			{
				using (var tempSession = Toolz.CreateChildSession(castedSession))
				{
					await (executeInSessionAsync(tempSession, cancellationToken)).ConfigureAwait(false);
					await (tempSession.FlushAsync(cancellationToken)).ConfigureAwait(false);
				}
			}
			else
			{
				await (executeInSessionAsync(castedSession, cancellationToken)).ConfigureAwait(false);
				// Explicity flushing the session, as the auto-flush may have already happened.
				await (castedSession.FlushAsync(cancellationToken)).ConfigureAwait(false);
			}
		}
	}
}
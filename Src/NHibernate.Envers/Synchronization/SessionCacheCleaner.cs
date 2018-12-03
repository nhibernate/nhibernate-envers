﻿using System.Threading;
using System.Threading.Tasks;
using NHibernate.Action;
using NHibernate.Event;

namespace NHibernate.Envers.Synchronization
{
	/// <summary>
	/// Class responsible for evicting audit data entries that have been stored in the session level cache. 
	/// This operation increases Envers performance in case of massive entity updates without clearing persistence context.
	/// </summary>
	public static class SessionCacheCleaner
	{
		/// <summary>
		/// Schedules audit data removal from session level cache after transaction completion. 
		/// The operation is performed regardless of commit success if session still is open.
		/// </summary>
		/// <param name="session">Active NHibernate session</param>
		/// <param name="data">Audit data that shall be evicted (e.g. revision data or entity snapshot)</param>
		public static void ScheduleAuditDataRemoval(ISession session, object data)
		{
			((IEventSource)session).ActionQueue.RegisterProcess(new cleanupAfterTranProcess(session, data));
		}

		private class cleanupAfterTranProcess : IAfterTransactionCompletionProcess
		{
			private readonly ISession _session;
			private readonly object _data;

			public cleanupAfterTranProcess(ISession session, object data)
			{
				_session = session;
				_data = data;
			}
			
			public void ExecuteAfterTransactionCompletion(bool success)
			{
				if(_session.IsOpen)
				{
					_session.Evict(_data);
				}
			}

			public Task ExecuteAfterTransactionCompletionAsync(bool success, CancellationToken cancellationToken)
			{
				ExecuteAfterTransactionCompletion(success);
				return Task.CompletedTask;
			}
		}
	}
}
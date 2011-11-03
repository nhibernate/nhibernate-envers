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
			((IEventSource)session).ActionQueue.RegisterProcess(success =>
			                                                    	{
																						if(session.IsOpen)
																						{
																							session.Evict(data);
																						}
			                                                    	});
		}
	}
}
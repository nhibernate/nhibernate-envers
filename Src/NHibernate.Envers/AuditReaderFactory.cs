using System.Linq;
using NHibernate.Engine;
using NHibernate.Envers.Event;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers
{
	public static class AuditReaderFactory
	{
		/// <summary>
		/// Create an audit reader associated with an open session.
		/// </summary>
		/// <param name="session">An open session.</param>
		/// <returns>An audit reader associated with the given sesison. It shouldn't be used after the session is closed.</returns>
		/// <exception cref="AuditException">When the given required listeners aren't installed.</exception>
		public static IAuditReader Get(ISession session)
		{
			var sessionImpl = session as ISessionImplementor
						?? (ISessionImplementor)session.SessionFactory.GetCurrentSession();

			var listeners = sessionImpl.Listeners;

			foreach (var listener in listeners.PostInsertEventListeners)
			{
				var auditEventListener = listener as AuditEventListener;
				if (auditEventListener == null) continue;

				var auditEventListenerType = typeof(AuditEventListener);
				if (listeners.PostUpdateEventListeners.Any(auditEventListenerType.IsInstanceOfType) &&
					 listeners.PostDeleteEventListeners.Any(auditEventListenerType.IsInstanceOfType))
				{
					return new AuditReader(auditEventListener.VerCfg, session, sessionImpl);
				}
			}

			throw new AuditException("You need to install NHibernate.Envers.Event.AuditEventListener " +
					"class as post insert, update and delete event listener.");
		}

		/// <summary>
		/// Create an audit reader associated with an open session.
		/// </summary>
		/// <param name="session">An open session.</param>
		/// <returns>An audit reader associated with the given sesison. It shouldn't be used after the session is closed.</returns>
		/// <exception cref="AuditException">When the given required listeners aren't installed.</exception>
		public static IAuditReader Auditer(this ISession session)
		{
			return Get(session);
		}
	}
}

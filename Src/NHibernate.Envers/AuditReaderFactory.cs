using NHibernate.Engine;
using NHibernate.Envers.Event;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

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
			var sessionImpl = session as ISessionImplementor;
			if (sessionImpl == null)
			{
				//rk - why is this needed?
				//fm = it is an interesting feature when you are using contextualized sessions (as I'm doing always)
				sessionImpl = (ISessionImplementor)session.SessionFactory.GetCurrentSession();
			}

			var listeners = sessionImpl.Listeners;

			foreach (var listener in listeners.PostInsertEventListeners)
			{
				var auditEventListener = listener as AuditEventListener;
				if (auditEventListener!=null) 
				{
					if (ArraysTools.ArrayIncludesInstanceOf(listeners.PostUpdateEventListeners, typeof(AuditEventListener)) &&
						ArraysTools.ArrayIncludesInstanceOf(listeners.PostDeleteEventListeners, typeof(AuditEventListener))) 
					{
							return new AuditReader(auditEventListener.VerCfg, session,sessionImpl);
					}
				}
			}

			throw new AuditException("You need to install nhibernate.envers.event.AuditEventListener " +
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

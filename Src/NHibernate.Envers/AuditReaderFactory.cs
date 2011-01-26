using NHibernate.Engine;
using NHibernate.Envers.Event;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers
{
	public static class AuditReaderFactory 
	{

		/**
		 * Create an audit reader associated with an open session.
		 * @param session An open session.
		 * @return An audit reader associated with the given sesison. It shouldn't be used
		 * after the session is closed.
		 * @throws AuditException When the given required listeners aren't installed.
		 */
		public static IAuditReader Get(ISession session)
		{
			var sessionImpl = session as ISessionImplementor;
			if (sessionImpl == null)
			{
				//rk - why is this needed?
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
							return new AuditReader(auditEventListener.getVerCfg(), session,sessionImpl);
					}
				}
			}

			throw new AuditException("You need to install nhibernate.envers.event.AuditEventListener " +
					"class as post insert, update and delete event listener.");
		}
	}
}

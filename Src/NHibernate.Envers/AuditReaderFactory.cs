using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Envers.Event;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Event;

namespace NHibernate.Envers
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class AuditReaderFactory {
        private AuditReaderFactory() { }

        /**
         * Create an audit reader associated with an open session.
         * @param session An open session.
         * @return An audit reader associated with the given sesison. It shouldn't be used
         * after the session is closed.
         * @throws AuditException When the given required listeners aren't installed.
         */
        public static IAuditReader Get(ISession session){
            ISessionImplementor sessionImpl;
		    if (!(session is ISessionImplementor)) {
			    sessionImpl = (ISessionImplementor) session.SessionFactory.GetCurrentSession();
		    } else {
			    sessionImpl = (ISessionImplementor) session;
		    }

            EventListeners listeners = sessionImpl.Listeners;

            foreach (IPostInsertEventListener listener in listeners.PostInsertEventListeners) {
                if (listener is AuditEventListener) {
                    if (ArraysTools.ArrayIncludesInstanceOf(listeners.PostUpdateEventListeners, typeof(AuditEventListener)) &&
                        ArraysTools.ArrayIncludesInstanceOf(listeners.PostDeleteEventListeners, typeof(AuditEventListener))) {
                        return new AuditReader(((AuditEventListener) listener).getVerCfg(), session,
                                sessionImpl);
                    }
                }
            }

            throw new AuditException("You need to install the org.hibernate.envers.event.AuditEventListener " +
                    "class as post insert, update and delete event listener.");
        }

        /**
         * Create an audit reader associated with an open entity manager.
         * @param entityManager An open entity manager.
         * @return An audit reader associated with the given entity manager. It shouldn't be used
         * after the entity manager is closed.
         * @throws AuditException When the given entity manager is not based on Hibernate, or if the required
         * listeners aren't installed.
         */
        //TODO Simon See if need it with NHibernate
        /*
        public static IAuditReader Get(EntityManager entityManager){
            if (entityManager.getDelegate() is ISession) {
                return get((ISession) entityManager.getDelegate());
            }

            if (entityManager.getDelegate() is EntityManager) {
                return get((EntityManager) entityManager.getDelegate());
            }

            throw new AuditException("Hibernate EntityManager not present!");
        }*/
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools;
using System.Reflection;

namespace NHibernate.Envers.RevisionInfo
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class DefaultRevisionInfoGenerator : IRevisionInfoGenerator {
        private readonly String revisionInfoEntityName;
        private readonly NewRevisionDelegate newRevisionDelegate;
        private readonly System.Type revisionInfoType;
        private readonly PropertyData revisionInfoTimestampData;

        public DefaultRevisionInfoGenerator(String revisionInfoEntityName, System.Type revisionInfoT,
                                           //Class<? extends RevisionListener> listenerClass,
                                           PropertyData revisionInfoTimestampData
                                           /*, bool timestampAsDate */) {
            this.revisionInfoEntityName = revisionInfoEntityName;
            this.revisionInfoType = revisionInfoT;
            this.revisionInfoTimestampData = revisionInfoTimestampData;

            
            //if (!listenerClass.equals(RevisionListener.class)) {
            //    // This is not the default value.
            //    try {
            //        evtDelegate = listenerClass.newInstance();
            //    } catch (InstantiationException e) {
            //        throw new MappingException(e);
            //    } catch (IllegalAccessException e) {
            //        throw new MappingException(e);
            //    }
            //} else {
                // Default evtDelegate - none
                newRevisionDelegate = null;
            //}
        }

	    public void saveRevisionData(ISession session, Object revisionData) {
            session.Save(revisionInfoEntityName, revisionData);
	    }

        public Object generate() {
		    Object revisionInfo;
            revisionInfo = Activator.CreateInstance(revisionInfoType);
            
            PropertyInfo prop = revisionInfoType.GetProperty(revisionInfoTimestampData.BeanName, typeof(DateTime));

            prop.SetValue(revisionInfo, DateTime.Now, null);

            if (newRevisionDelegate != null)
            {
                newRevisionDelegate(revisionInfo);
            }

            return revisionInfo;
        }
    }
}

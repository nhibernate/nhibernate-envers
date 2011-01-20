using System;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
    public class DefaultRevisionInfoGenerator : IRevisionInfoGenerator 
	{
        private readonly String _revisionInfoEntityName;
        private readonly System.Type _revisionInfoType;
    	private readonly bool _timestampAsDate;
    	private readonly IRevisionListener _listener;
    	private readonly ISetter _revisionTimestampSetter;

    	public DefaultRevisionInfoGenerator(String revisionInfoEntityName, System.Type revisionInfoType,
                                           System.Type listenerClass,
                                           PropertyData revisionInfoTimestampData,
                                           bool timestampAsDate) 
		{
            _revisionInfoEntityName = revisionInfoEntityName;
            _revisionInfoType = revisionInfoType;
    		_timestampAsDate = timestampAsDate;

    		_revisionTimestampSetter = ReflectionTools.GetSetter(revisionInfoType, revisionInfoTimestampData);

			if(!listenerClass.Equals(typeof(IRevisionListener)))
			{
				_listener = (IRevisionListener)Activator.CreateInstance(listenerClass);
			}
        }

	    public void saveRevisionData(ISession session, object revisionData) 
		{
            session.Save(_revisionInfoEntityName, revisionData);
	    }

        public object generate() 
		{
        	var revisionInfo = Activator.CreateInstance(_revisionInfoType);

        	var value = _timestampAsDate ? (object) DateTime.Now : DateTime.Now.Ticks;

        	_revisionTimestampSetter.Set(revisionInfo, value);

            if (_listener != null)
            {
                _listener.NewRevision(revisionInfo);
            }

            return revisionInfo;
        }
    }
}

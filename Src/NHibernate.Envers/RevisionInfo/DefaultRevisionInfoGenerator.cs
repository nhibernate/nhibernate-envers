using System;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Synchronization;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	[Serializable]
	public partial class DefaultRevisionInfoGenerator : IRevisionInfoGenerator
	{
		private readonly string _revisionInfoEntityName;
		private readonly System.Type _revisionInfoType;
		private readonly bool _timestampAsDate;
		private readonly IRevisionListener _listener;
		private readonly ISetter _revisionTimestampSetter;

		public DefaultRevisionInfoGenerator(string revisionInfoEntityName,
											System.Type revisionInfoType,
											IRevisionListener revisionListener,
											PropertyData revisionInfoTimestampData,
											bool timestampAsDate)
		{
			_revisionInfoEntityName = revisionInfoEntityName;
			_revisionInfoType = revisionInfoType;
			_timestampAsDate = timestampAsDate;
			_revisionTimestampSetter = ReflectionTools.GetSetter(revisionInfoType, revisionInfoTimestampData);
			_listener = revisionListener;
		}

		public void SaveRevisionData(ISession session, object revisionData)
		{
			session.Save(_revisionInfoEntityName, revisionData);
			SessionCacheCleaner.ScheduleAuditDataRemoval(session, revisionData);
		}

		public object Generate()
		{
			var revisionInfo = Activator.CreateInstance(_revisionInfoType);

			var utcNow = DateTime.UtcNow;
			var value = _timestampAsDate ? (object)utcNow : utcNow.Ticks;
			_revisionTimestampSetter.Set(revisionInfo, value);

			_listener?.NewRevision(revisionInfo);

			return revisionInfo;
		}

		public virtual void EntityChanged(System.Type entityClass, string entityName, object entityId, RevisionType revisionType, object revisionEntity)
		{
			var castedListener = _listener as IEntityTrackingRevisionListener;
			castedListener?.EntityChanged(entityClass, entityName, entityId, revisionType, revisionEntity);
		}
	}
}

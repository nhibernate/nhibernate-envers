using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;

namespace NHibernate.Envers.RevisionInfo
{
	[Serializable]
	public class RevisionInfoQueryCreator
	{
		private readonly string _revisionInfoEntityName;
		private readonly string _revisionInfoIdName;
		private readonly string _revisionInfoTimestampName;
		private readonly bool _timestampAsDate;
		private readonly System.Type _revisionType;

		public RevisionInfoQueryCreator(string revisionInfoEntityName,
										string revisionInfoIdName,
										string revisionInfoTimestampName,
										bool timestampAsDate,
										System.Type revisionType)
		{
			_revisionInfoEntityName = revisionInfoEntityName;
			_revisionInfoIdName = revisionInfoIdName;
			_revisionInfoTimestampName = revisionInfoTimestampName;
			_timestampAsDate = timestampAsDate;
			_revisionType = revisionType;
		}

		public ICriteria RevisionDateQuery(ISession session, long revision)
		{
			//seems that criteria needs exact type
			object castedRevision = revision;
			if (_revisionType == typeof(int))
				castedRevision = (int)revision;

			return session.CreateCriteria(_revisionInfoEntityName)
					.SetProjection(Projections.Property(_revisionInfoTimestampName))
					.Add(Restrictions.Eq(_revisionInfoIdName, castedRevision));
		}

		public ICriteria RevisionNumberForDateQuery(ISession session, DateTime date)
		{
			return session.CreateCriteria(_revisionInfoEntityName)
					.SetProjection(Projections.Max(_revisionInfoIdName))
					.Add(Restrictions.Le(_revisionInfoTimestampName, _timestampAsDate ? (object)date : date.Ticks));
		}

		public ICriteria RevisionsQuery(ISession session, IEnumerable<long> revisions)
		{
			return session.CreateCriteria(_revisionInfoEntityName)
				.Add(Restrictions.In(_revisionInfoIdName, revisions.ToArray()));
		}
	}
}

using System;
using System.Text;

namespace NHibernate.Envers.RevisionInfo
{
	public class RevisionInfoQueryCreator 
	{
		private readonly bool _timestampAsDate;
		private readonly string revisionDateQuery;
		private readonly string revisionNumberForDateQuery;
		private readonly string revisionQuery;

		public RevisionInfoQueryCreator(string revisionInfoEntityName, 
										string revisionInfoIdName,
										string revisionInfoTimestampName,
										bool timestampAsDate) 
		{
			_timestampAsDate = timestampAsDate;

			revisionDateQuery = new StringBuilder()
					.Append("select rev.").Append(revisionInfoTimestampName)
					.Append(" from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoIdName).Append(" = :_revision_number")
					.ToString();

			revisionNumberForDateQuery = new StringBuilder()
					.Append("select max(rev.").Append(revisionInfoIdName)
					.Append(") from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoTimestampName).Append(" <= :_revision_date")
					.ToString();

			revisionQuery = new StringBuilder()
					.Append("select rev from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoIdName)
					.Append(" = :_revision_number")
					.ToString();
		}

		public IQuery RevisionDateQuery(ISession session, long revision) 
		{
			return session.CreateQuery(revisionDateQuery).SetInt64("_revision_number", revision);
		}

		public IQuery RevisionNumberForDateQuery(ISession session, DateTime date) 
		{
			//rk: todo - fix this
			return session.CreateQuery(revisionNumberForDateQuery).SetParameter("_revision_date", _timestampAsDate ? (object) date : new TimeSpan(date.Ticks));
		}

		public IQuery RevisionQuery(ISession session, long revision) 
		{
			return session.CreateQuery(revisionQuery).SetInt64("_revision_number", revision);
		}
	}
}

using System;
using System.Text;

namespace NHibernate.Envers.RevisionInfo
{
	public class RevisionInfoQueryCreator 
	{
		private const string RevisionNumberParameterName = "_revision_number";
		private const string RevisionDateParameterName = "_revision_date";
		private readonly bool timestampAsDate;
		private readonly string revisionDateQuery;
		private readonly string revisionNumberForDateQuery;
		private readonly string revisionQuery;

		public RevisionInfoQueryCreator(string revisionInfoEntityName, 
										string revisionInfoIdName,
										string revisionInfoTimestampName,
										bool timestampAsDate) 
		{
			this.timestampAsDate = timestampAsDate;

			revisionDateQuery = new StringBuilder(512)
					.Append("select rev.").Append(revisionInfoTimestampName)
					.Append(" from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoIdName).Append(" = :").Append(RevisionNumberParameterName)
					.ToString();

			revisionNumberForDateQuery = new StringBuilder(512)
					.Append("select max(rev.").Append(revisionInfoIdName)
					.Append(") from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoTimestampName).Append(" <= :").Append(RevisionDateParameterName)
					.ToString();

			revisionQuery = new StringBuilder(512)
					.Append("select rev from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoIdName)
					.Append(" = :").Append(RevisionNumberParameterName)
					.ToString();
		}

		public IQuery RevisionDateQuery(ISession session, long revision) 
		{
			return session.CreateQuery(revisionDateQuery).SetInt64(RevisionNumberParameterName, revision);
		}

		public IQuery RevisionNumberForDateQuery(ISession session, DateTime date) 
		{
			return session.CreateQuery(revisionNumberForDateQuery).SetParameter(RevisionDateParameterName, timestampAsDate ? (object) date : date.Ticks);
		}

		public IQuery RevisionQuery(ISession session, long revision) 
		{
			return session.CreateQuery(revisionQuery).SetInt64(RevisionNumberParameterName, revision);
		}
	}
}

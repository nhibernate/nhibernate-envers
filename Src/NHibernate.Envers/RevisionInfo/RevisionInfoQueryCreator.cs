using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Envers.RevisionInfo
{
	public class RevisionInfoQueryCreator
	{
		private const string RevisionNumbersParameterName = "_revision_numbers";
		private const string RevisionNumberForDateParamaterName = "_revision_number";
		private const string RevisionDateParameterName = "_revision_date";
		private readonly string entitiesChangedInRevisionQuery;
		private readonly bool timestampAsDate;
		private readonly string revisionDateQuery;
		private readonly string revisionNumberForDateQuery;
		private readonly string revisionsQuery;

		public RevisionInfoQueryCreator(string revisionInfoEntityName,
										string revisionInfoIdName,
										string revisionInfoTimestampName,
										bool timestampAsDate,
										string modifiedEntityNamesName)
		{
			this.timestampAsDate = timestampAsDate;

			revisionDateQuery = new StringBuilder(512)
					.Append("select rev.").Append(revisionInfoTimestampName)
					.Append(" from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoIdName).Append(" = :").Append(RevisionNumberForDateParamaterName)
					.ToString();

			revisionNumberForDateQuery = new StringBuilder(512)
					.Append("select max(rev.").Append(revisionInfoIdName)
					.Append(") from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoTimestampName).Append(" <= :").Append(RevisionDateParameterName)
					.ToString();

			revisionsQuery = new StringBuilder(512)
					.Append("select rev from ").Append(revisionInfoEntityName)
					.Append(" rev where ").Append(revisionInfoIdName)
					.Append(" in (:" + RevisionNumbersParameterName + ")")
					.ToString();

			entitiesChangedInRevisionQuery = new StringBuilder(512)
					.Append("select elements(rev.").Append(modifiedEntityNamesName)
					.Append(") from ").Append(revisionInfoEntityName)
					.Append(" rev where rev.").Append(revisionInfoIdName).Append(" = :_revision_number")
					.ToString();
		}

		public IQuery RevisionDateQuery(ISession session, long revision)
		{
			return session.CreateQuery(revisionDateQuery).SetInt64(RevisionNumberForDateParamaterName, revision);
		}

		public IQuery RevisionNumberForDateQuery(ISession session, DateTime date)
		{
			return session.CreateQuery(revisionNumberForDateQuery).SetParameter(RevisionDateParameterName, timestampAsDate ? (object)date : date.Ticks);
		}

		public IQuery RevisionsQuery(ISession session, IEnumerable<long> revisions)
		{
			return session.CreateQuery(revisionsQuery).SetParameterList(RevisionNumbersParameterName, revisions);
		}

		public IQuery EntitiesChangedInRevisionQuery(ISession session, long revision)
		{
			return session.CreateQuery(entitiesChangedInRevisionQuery).SetParameter("_revision_number", revision);
		}
	}
}

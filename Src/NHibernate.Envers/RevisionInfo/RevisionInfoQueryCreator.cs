using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.RevisionInfo
{
/**
 * @author Adam Warski (adam at warski dot org)
 */
public class RevisionInfoQueryCreator {
    private readonly String revisionDateQuery;
    private readonly String revisionNumberForDateQuery;
    private readonly String revisionQuery;

    public RevisionInfoQueryCreator(String revisionInfoEntityName, String revisionInfoIdName,
                                    String revisionInfoTimestampName) {
        
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

    public NHibernate.IQuery getRevisionDateQuery(ISession session, long revision) {
        return session.CreateQuery(revisionDateQuery).SetParameter("_revision_number", revision);
    }

    public NHibernate.IQuery getRevisionNumberForDateQuery(ISession session, DateTime date) {
        return session.CreateQuery(revisionNumberForDateQuery).SetParameter("_revision_date", date);
    }

    public NHibernate.IQuery getRevisionQuery(ISession session, long revision) {
        return session.CreateQuery(revisionQuery).SetParameter("_revision_number", revision);
    }
}
}

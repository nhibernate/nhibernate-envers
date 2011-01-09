using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
    public interface IAuditCriterion
    {
        void AddToQuery(AuditConfiguration auditCfg, String entityName, QueryBuilder qb, Parameters parameters);
    }
}

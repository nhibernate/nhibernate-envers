using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Order
{
    public interface IAuditOrder
    {
        /**
         * @param auditCfg Configuration.
         * @return A pair: (property name, ascending?).
         */
        Pair<String, Boolean> GetData(AuditConfiguration auditCfg);
    }
}

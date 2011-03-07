using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Order
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public interface IAuditOrder
    {
        /**
         * @param auditCfg Configuration.
         * @return A pair: (property name, ascending?).
         */
        Pair<String, Boolean> getData(AuditConfiguration auditCfg);
    }
}

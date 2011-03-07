using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Projection
{
    public interface IAuditProjection
    {
        /**
         * @param auditCfg Configuration.
         * @return A triple: (function name - possibly null, property name, add distinct?).
         */
        Triple<String, String, Boolean> GetData(AuditConfiguration auditCfg);
    }
}

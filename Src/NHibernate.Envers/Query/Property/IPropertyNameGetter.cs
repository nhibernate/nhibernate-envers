using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
    /**
     * Provides a function to get the name of a property, which is used in a query, to apply some restrictions on it.
     * @author Adam Warski (adam at warski dot org)
     */
    public interface IPropertyNameGetter
    {
        /**
         * @param auditCfg Audit configuration.
         * @return Name of the property, to be used in a query.
         */
        String Get(AuditConfiguration auditCfg);
    }
}

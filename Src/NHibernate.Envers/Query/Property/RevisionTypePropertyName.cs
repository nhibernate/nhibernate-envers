using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
    /**
     * Used for specifying restrictions on the revision number, corresponding to an audit entity.
     * @author Adam Warski (adam at warski dot org)
     */
    public class RevisionTypePropertyName : IPropertyNameGetter {
        public String Get(AuditConfiguration auditCfg) {
            return auditCfg.AuditEntCfg.RevisionTypePropName;
        }
    }
}

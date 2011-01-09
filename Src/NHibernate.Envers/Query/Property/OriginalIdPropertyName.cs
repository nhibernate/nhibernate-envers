using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
    /**
     * Used for specifying restrictions on the identifier.
     * TODO: idPropertyName should be read basing on auditCfg + entityName
     * @author Adam Warski (adam at warski dot org)
     */
    public class OriginalIdPropertyName : IPropertyNameGetter {
        private readonly String idPropertyName;

        public OriginalIdPropertyName(String idPropertyName) {
            this.idPropertyName = idPropertyName;
        }

        public String Get(AuditConfiguration auditCfg) {
            return auditCfg.AuditEntCfg.OriginalIdPropName + "." + idPropertyName;
        }
    }
}

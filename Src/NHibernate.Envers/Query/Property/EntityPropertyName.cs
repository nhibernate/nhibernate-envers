using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
    /**
     * Used for specifying restrictions on a property of an audited entity.
     * @author Adam Warski (adam at warski dot org)
     */
    public class EntityPropertyName : IPropertyNameGetter {
        private readonly String propertyName;

        public EntityPropertyName(String propertyName) {
            this.propertyName = propertyName;
        }

        public String Get(AuditConfiguration auditCfg) {
            return propertyName;
        }
    }
}

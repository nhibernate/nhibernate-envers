using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
    /**
     * Used for specifying restrictions on a property of the revision entity, which is associated with an audit entity.
     * @author Adam Warski (adam at warski dot org)
     */
    public class RevisionPropertyPropertyName : IPropertyNameGetter {
        private readonly String propertyName;

        public RevisionPropertyPropertyName(String propertyName) {
            this.propertyName = propertyName;
        }

        public String Get(AuditConfiguration auditCfg) {
            return auditCfg.AuditEntCfg.GetRevisionPropPath(propertyName);
        }
    }
}

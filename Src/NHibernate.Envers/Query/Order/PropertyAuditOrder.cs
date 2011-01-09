using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Order
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class PropertyAuditOrder : IAuditOrder {
        private readonly IPropertyNameGetter propertyNameGetter;
        private readonly bool asc;

        public PropertyAuditOrder(IPropertyNameGetter propertyNameGetter, bool asc) {
            this.propertyNameGetter = propertyNameGetter;
            this.asc = asc;
        }

        public Pair<String, Boolean> getData(AuditConfiguration auditCfg) {
            return Pair<String, Boolean>.Make(propertyNameGetter.Get(auditCfg), asc);
        }
    }
}

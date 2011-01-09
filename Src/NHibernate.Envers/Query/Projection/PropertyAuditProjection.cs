using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Projection
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class PropertyAuditProjection : IAuditProjection {
        private readonly IPropertyNameGetter propertyNameGetter;
        private readonly String function;
        private readonly bool distinct;

        public PropertyAuditProjection(IPropertyNameGetter propertyNameGetter, String function, bool distinct) {
            this.propertyNameGetter = propertyNameGetter;
            this.function = function;
            this.distinct = distinct;
        }

        public Triple<String, String, Boolean> GetData(AuditConfiguration auditCfg) {
            String propertyName = propertyNameGetter.Get(auditCfg);

            return Triple<String, String, Boolean>.Make(function, propertyName, distinct);
        }
    }
}

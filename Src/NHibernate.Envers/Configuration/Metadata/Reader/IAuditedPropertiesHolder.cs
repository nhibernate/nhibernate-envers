using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
    /**
     * Implementations hold other audited properties.
     * @author Simon Duduica, port of Envers Tools class by Adam Warski (adam at warski dot org)
     */
    public interface IAuditedPropertiesHolder
    {
        /**
         * Add an audited property.
         * @param propertyName Name of the audited property.
         * @param auditingData Data for the audited property.
         */
        void AddPropertyAuditingData(String propertyName, PropertyAuditingData auditingData);

        /**
         * @param propertyName Name of a property.
         * @return Auditing data for the property.
         */
        PropertyAuditingData GetPropertyAuditingData(String propertyName);
    }
}

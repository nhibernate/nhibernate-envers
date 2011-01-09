using System;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
    public class ComponentAuditingData : PropertyAuditingData, IAuditedPropertiesHolder 
	{
	    private readonly IDictionary<String, PropertyAuditingData> properties;

	    public ComponentAuditingData() 
		{
		    properties = new Dictionary<String, PropertyAuditingData>();
	    }

	    public void AddPropertyAuditingData(String propertyName, PropertyAuditingData auditingData) 
		{
		    properties.Add(propertyName, auditingData);
	    }

        public PropertyAuditingData GetPropertyAuditingData(String propertyName)
        {
        	PropertyAuditingData ret;
        	properties.TryGetValue(propertyName, out ret);
        	return ret;
        }
    }
}

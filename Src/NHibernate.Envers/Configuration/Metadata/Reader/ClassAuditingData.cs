using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
    /**
     * @author Simon Duduica, port of Envers Tools class by Adam Warski (adam at warski dot org)
     * @author Sebastian Komander
    */
    public class ClassAuditingData : IAuditedPropertiesHolder {
        public IDictionary<String, PropertyAuditingData> Properties { get; private set; }
        public IDictionary<String, String> SecondaryTableDictionary { get; private set; }

        public AuditTableAttribute AuditTable { get; set; }

	    /**
	     * True if the class is audited globally (this helps to cover the cases when there are no fields in the class,
	     * but it's still audited).
	     */
	    private bool defaultAudited;

        public ClassAuditingData() {
            Properties = Toolz.NewDictionary<String,PropertyAuditingData>();
            SecondaryTableDictionary = Toolz.NewDictionary<String, String>();
        }

	    public void AddPropertyAuditingData(String propertyName, PropertyAuditingData auditingData) {
		    Properties.Add(propertyName, auditingData);
	    }

        public PropertyAuditingData GetPropertyAuditingData(String propertyName) {
            try{
                return Properties[propertyName];
            }
            catch(KeyNotFoundException){
                return null;
            }
        }

        public IEnumerable<String> getPropertyNames() {
            return Properties.Keys;
        }

	    public void setDefaultAudited(bool defaultAudited) {
		    this.defaultAudited = defaultAudited;
	    }

	    public bool IsAudited() {
            return defaultAudited || Properties.Count > 0;
        }
    }
}

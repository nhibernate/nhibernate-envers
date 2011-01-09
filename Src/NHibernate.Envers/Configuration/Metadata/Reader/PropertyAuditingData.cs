using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Entities;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class PropertyAuditingData {
        public String Name { get; set; }
        public String BeanName { get; set; }
        public ModificationStore Store { get; set; }
        public String MapKey { get; set; }
        public AuditJoinTableAttribute JoinTable { get; set; }
        public String AccessType { get; set; }
        private IList<AuditOverrideAttribute> auditJoinTableOverrides = new List<AuditOverrideAttribute>(0);
        public IList<AuditOverrideAttribute> AuditingOverrides {get{return auditJoinTableOverrides;}}
	    private RelationTargetAuditMode relationTargetAuditMode;
        public String AuditMappedBy { get; set; }
        public String PositionMappedBy { get; set; }
        public bool ForceInsertable { get; set; }

	    public PropertyAuditingData() {
        }

        public PropertyAuditingData(String name, String accessType, ModificationStore store,
								    RelationTargetAuditMode relationTargetAuditMode,
                                    String auditMappedBy, String positionMappedBy,
                                    bool forceInsertable) {
            this.Name = name;
		    this.BeanName = name;
            this.AccessType = accessType;
            this.Store = store;
		    this.relationTargetAuditMode = relationTargetAuditMode;
            this.AuditMappedBy = auditMappedBy;
            this.PositionMappedBy = positionMappedBy;
            this.ForceInsertable = forceInsertable;
        }


        public PropertyData getPropertyData() {
            return new PropertyData(Name, BeanName, AccessType, Store);
        }

        public void addAuditingOverride(AuditOverrideAttribute annotation) {
		    if (annotation != null) {
			    String overrideName = annotation.Name;
			    bool present = false;
			    foreach (AuditOverrideAttribute current in auditJoinTableOverrides) {
				    if (current.Name.Equals(overrideName)) {
					    present = true;
					    break;
				    }
			    }
			    if (!present) {
				    auditJoinTableOverrides.Add(annotation);
			    }
		    }
	    }

	    public void addAuditingOverrides(AuditOverridesAttribute annotationOverrides) {
		    if (annotationOverrides != null) {
			    foreach (AuditOverrideAttribute attrib in annotationOverrides.value) {
				    addAuditingOverride(attrib);
			    }
		    }
	    }

	    /**
	     * Get the relationTargetAuditMode property.
	     *
	     * @return the relationTargetAuditMode property value
	     */
	    public RelationTargetAuditMode getRelationTargetAuditMode() {
		    return relationTargetAuditMode;
	    }

	    /**
	     * Set the relationTargetAuditMode property value.
	     *
	     * @param relationTargetAuditMode the relationTargetAuditMode to set
	     */
	    public void setRelationTargetAuditMode(RelationTargetAuditMode relationTargetAuditMode) {
		    this.relationTargetAuditMode = relationTargetAuditMode;
	    }

    }
}

using System;
using System.Collections.Generic;
using NHibernate.Envers.Tools;
using NHibernate.Mapping;
using System.Reflection;
using NHibernate.Envers.Compatibility.Attributes;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
    /**
     * Reads persistent properties form a
     * {@link org.hibernate.envers.configuration.metadata.reader.PersistentPropertiesSource}
     * and adds the ones that are audited to a
     * {@link org.hibernate.envers.configuration.metadata.reader.AuditedPropertiesHolder},
     * filling all the auditing data.
     * @author Simon Duduica, port of Envers Tools class by Adam Warski (adam at warski dot org)
     * @author Erik-Berndt Scheper
     */
    public class AuditedPropertiesReader 
	{
    	private readonly PropertyAndMemberInfo _propertyAndMemberInfo;
    	private readonly ModificationStore _defaultStore;
	    private readonly IPersistentPropertiesSource _persistentPropertiesSource;
	    private readonly IAuditedPropertiesHolder _auditedPropertiesHolder;
	    private readonly GlobalConfiguration _globalCfg;
	    private readonly String _propertyNamePrefix;

	    public AuditedPropertiesReader(PropertyAndMemberInfo propertyAndMemberInfo,
										ModificationStore defaultStore,
										IPersistentPropertiesSource persistentPropertiesSource,
										IAuditedPropertiesHolder auditedPropertiesHolder,
										GlobalConfiguration globalCfg,
										string propertyNamePrefix) {
	    	_propertyAndMemberInfo = propertyAndMemberInfo;
	    	_defaultStore = defaultStore;
		    _persistentPropertiesSource = persistentPropertiesSource;
		    _auditedPropertiesHolder = auditedPropertiesHolder;
		    _globalCfg = globalCfg;
		    _propertyNamePrefix = propertyNamePrefix;
	    }

		public void read()
		{
			// Adding all properties from the given class.
			AddPropertiesFromClass(_persistentPropertiesSource.Clazz);
		}

		private void AddPropertiesFromClass(System.Type clazz)
		{
			foreach (var declaredPersistentProperty in _propertyAndMemberInfo.GetPersistentInfo(clazz, _persistentPropertiesSource.PropertyEnumerator))
			{
				IValue propertyValue = declaredPersistentProperty.Property.Value;

				PropertyAuditingData propertyData;
				bool isAudited;
				if (propertyValue is Component)
				{
					var componentData = new ComponentAuditingData();
					isAudited = FillPropertyData(declaredPersistentProperty.Member,
                                                    declaredPersistentProperty.Property.Name,
                                                    componentData,
					                                declaredPersistentProperty.Property.PropertyAccessorName);

					IPersistentPropertiesSource componentPropertiesSource = new ComponentPropertiesSource(
						(Component) propertyValue);
					new AuditedPropertiesReader(_propertyAndMemberInfo, 
												ModificationStore.FULL, componentPropertiesSource, componentData,
					                            _globalCfg,
					                            _propertyNamePrefix +
					                            MappingTools.createComponentPrefix(declaredPersistentProperty.Property.Name))
						.read();

					propertyData = componentData;
				}
				else
				{
					propertyData = new PropertyAuditingData();
                    isAudited = FillPropertyData(declaredPersistentProperty.Member, 
                                                    declaredPersistentProperty.Property.Name, 
                                                    propertyData,
					                                declaredPersistentProperty.Property.PropertyAccessorName);
				}

				if (isAudited)
				{
					// Now we know that the property is audited
					_auditedPropertiesHolder.AddPropertyAuditingData(declaredPersistentProperty.Property.Name, propertyData);
				}
			}
		}

    	/**
	     * Checks if a property is audited and if yes, fills all of its data.
	     * @param property Property to check.
	     * @param propertyData Property data, on which to set this property's modification store.
	     * @param accessType Access type for the property.
	     * @return False if this property is not audited.
	     */
	    private bool FillPropertyData(MemberInfo property,
                                        string mappedPropertyName,
                                        PropertyAuditingData propertyData,
									    string accessType) 
        {

		    // check if a property is declared as not audited to exclude it
		    // useful if a class is audited but some properties should be excluded
            var unVer = (NotAuditedAttribute)Attribute.GetCustomAttribute(property, typeof(NotAuditedAttribute));
		    if (unVer != null) 
            {
			    return false;
		    }
	        // if the optimistic locking field has to be unversioned and the current property
	        // is the optimistic locking field, don't audit it
	        if (_globalCfg.DoNotAuditOptimisticLockingField) 
            {
				if (_persistentPropertiesSource.VersionedProperty != null)
				{
					if (_persistentPropertiesSource.VersionedProperty.Name.Equals(mappedPropertyName))
						return false;
				}
	            //Version jpaVer = property.getAnnotation(typeof(Version));
				//if (mappedVersionName.Equals(mappedPropertyName))
				//    return false;
				//var jpaVer = (VersionAttribute)Attribute.GetCustomAttribute(property, typeof(VersionAttribute));
				//if (jpaVer != null) {
				//    return false;
				//}
	        }

	        // Checking if this property is explicitly audited or if all properties are.
		    //AuditedAttribute aud = property.getAnnotation(typeof(AuditedAttribute));
            var aud = (AuditedAttribute)Attribute.GetCustomAttribute(property, typeof(AuditedAttribute));
		    if (aud != null) {
			    propertyData.Store = aud.ModStore;
			    propertyData.setRelationTargetAuditMode(aud.TargetAuditMode);
		    } else {
			    if (_defaultStore != ModificationStore._NULL) {
				    propertyData.Store = _defaultStore;
			    } else {
				    return false;
			    }
		    }

		    propertyData.Name = _propertyNamePrefix + mappedPropertyName;
            propertyData.BeanName = mappedPropertyName;
		    propertyData.AccessType = accessType;

		    AddPropertyJoinTables(property, propertyData);
		    AddPropertyAuditingOverrides(property, propertyData);
		    if (!ProcessPropertyAuditingOverrides(property, propertyData)) {
			    return false; // not audited due to AuditOverride annotation
		    }
		    AddPropertyMapKey(property, propertyData);
            SetPropertyAuditMappedBy(property, propertyData);

		    return true;
	    }

        private void SetPropertyAuditMappedBy(MemberInfo property, PropertyAuditingData propertyData) 
        {
            var auditMappedBy = (AuditMappedByAttribute)Attribute.GetCustomAttribute(property, typeof(AuditMappedByAttribute));
            if (auditMappedBy != null) 
            {
		        propertyData.AuditMappedBy = auditMappedBy.MappedBy;
                if (!"".Equals(auditMappedBy.PositionMappedBy)) {
                    propertyData.PositionMappedBy = auditMappedBy.PositionMappedBy;
                }
            }
        }

        private void AddPropertyMapKey(MemberInfo property, PropertyAuditingData propertyData) 
        {
		    var mapKey = (MapKeyAttribute)Attribute.GetCustomAttribute(property, typeof(MapKeyAttribute));
		    if (mapKey != null) 
            {
			    propertyData.MapKey = mapKey.Name;
		    }
	    }

	    private void AddPropertyJoinTables(MemberInfo property, PropertyAuditingData propertyData)
        {
		    // first set the join table based on the AuditJoinTable annotation
		    var joinTable = (AuditJoinTableAttribute)Attribute.GetCustomAttribute(property, typeof(AuditJoinTableAttribute));;
		    propertyData.JoinTable = joinTable ?? DEFAULT_AUDIT_JOIN_TABLE;
	    }

	    /***
	     * Add the {@link org.hibernate.envers.AuditOverride} annotations.
	     *
	     * @param property the property being processed
	     * @param propertyData the Envers auditing data for this property
	     */
        private void AddPropertyAuditingOverrides(MemberInfo property, PropertyAuditingData propertyData)
        {
		    var annotationOverride = (AuditOverrideAttribute)Attribute.GetCustomAttribute(property, typeof(AuditOverrideAttribute));
		    if (annotationOverride != null) 
            {
			    propertyData.addAuditingOverride(annotationOverride);
		    }
		    var annotationOverrides = (AuditOverridesAttribute)Attribute.GetCustomAttribute(property, typeof(AuditOverridesAttribute));
		    if (annotationOverrides != null) 
            {
			    propertyData.addAuditingOverrides(annotationOverrides);
		    }
	    }

	    /**
	     * Process the {@link org.hibernate.envers.AuditOverride} annotations for this property.
	     *
	     * @param property
	     *            the property for which the {@link org.hibernate.envers.AuditOverride}
	     *            annotations are being processed
	     * @param propertyData
	     *            the Envers auditing data for this property
	     * @return {@code false} if isAudited() of the override annotation was set to
	     */
        private bool ProcessPropertyAuditingOverrides(MemberInfo property, PropertyAuditingData propertyData) 
        {
		    // if this property is part of a component, process all override annotations
		    if (_auditedPropertiesHolder is ComponentAuditingData) {
			    var overrides = ((ComponentAuditingData) _auditedPropertiesHolder).AuditingOverrides;
			    foreach (var ovr in overrides) 
                {
				    if (property.Name.Equals(ovr.Name))
				    {
				        // the override applies to this property
					    if (!ovr.IsAudited) 
                        {
						    return false;
					    }
				        if (ovr.AuditJoinTable != null) 
                        {
				            propertyData.JoinTable = ovr.AuditJoinTable;
				        }
				    }
                }
    			
		    }
		    return true;
	    }

	    private static readonly AuditJoinTableAttribute DEFAULT_AUDIT_JOIN_TABLE = new DefaultAuditJoinTableAttribute();

        private class ComponentPropertiesSource : IPersistentPropertiesSource 
		{
		    private readonly System.Type xclass;
		    private readonly Component component;

		    public ComponentPropertiesSource(Component component) 
			{
				xclass = component.ComponentClass;
			    this.component = component;
		    }

		    public IEnumerable<Property> PropertyEnumerator { get { return component.PropertyIterator; } }
		    public Property GetProperty(String propertyName) { return component.GetProperty(propertyName); }

        	public System.Type Clazz
        	{
        		get { return xclass; }
        	}

        	public Property VersionedProperty
        	{
				get { return null; }
        	}
        }
    }
}

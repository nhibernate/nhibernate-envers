using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tools;
using NHibernate.Mapping;
using System.Reflection;
using NHibernate.Envers.Compatibility.Attributes;
using NHibernate.Properties;

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
    public class AuditedPropertiesReader {
    	private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
    	private readonly ModificationStore _defaultStore;
	    private readonly IPersistentPropertiesSource _persistentPropertiesSource;
	    private readonly IAuditedPropertiesHolder _auditedPropertiesHolder;
	    private readonly GlobalConfiguration _globalCfg;
	    private readonly String _propertyNamePrefix;

    	private static readonly List<IFieldNamingStrategy> DefaultFieldNamningStrategies =
    		new List<IFieldNamingStrategy>
    			{
    				new CamelCaseStrategy(),
    				new CamelCaseUnderscoreStrategy(),
    				new LowerCaseStrategy(),
    				new LowerCaseUnderscoreStrategy(),
    				new PascalCaseUnderscoreStrategy(),
    				new PascalCaseMUnderscoreStrategy(),
    			};

	    public AuditedPropertiesReader(ModificationStore defaultStore,
								       IPersistentPropertiesSource persistentPropertiesSource,
								       IAuditedPropertiesHolder auditedPropertiesHolder,
								       GlobalConfiguration globalCfg,
								       String propertyNamePrefix) {
		    _defaultStore = defaultStore;
		    _persistentPropertiesSource = persistentPropertiesSource;
		    _auditedPropertiesHolder = auditedPropertiesHolder;
		    _globalCfg = globalCfg;
		    _propertyNamePrefix = propertyNamePrefix;
	    }

		public void read()
		{
			// Adding all properties from the given class.
			AddPropertiesFromClass(_persistentPropertiesSource.GetClass());
		}

    	private class DeclaredPersistentProperty
		{
			public MemberInfo Member { get; set; }
			public Property Property { get; set; }
		}

		private IEnumerable<DeclaredPersistentProperty> GetPersistentInfo(System.Type @class, IEnumerable<Property> properties)
		{
			// a persistent property can be anything including a noop "property" declared in the mapping
			// for query only. In this case I will apply some trick to get the MemberInfo.
			var candidateMembers =
				@class.GetFields(DefaultBindingFlags).Concat(@class.GetProperties(DefaultBindingFlags).Cast<MemberInfo>()).ToList();
			var candidateMembersNames = candidateMembers.Select(m => m.Name).ToList();
			foreach (var property in properties)
			{
				var exactMemberIdx = candidateMembersNames.IndexOf(property.Name);
				if (exactMemberIdx >= 0)
				{
					// No metter which is the accessor the audit-attribute should be in the property where available and not
					// to the member used to read-write the value. (This method work even for access="field").
					yield return new DeclaredPersistentProperty {Member = candidateMembers[exactMemberIdx], Property = property};
				}
				else
				{
					// try to find the field using field-name-strategy
					//
					// This part will run for:
					// 1) query only property (access="none" or access="noop")
					// 2) a strange case where the element <property> is declared with a "field.xyz" but only a field is used in the class. (Only God may know way)
					int exactFieldIdx = GetMemberIdxByFieldNamingStrategies(candidateMembersNames, property);
					if (exactFieldIdx >= 0)
					{
						yield return new DeclaredPersistentProperty { Member = candidateMembers[exactFieldIdx], Property = property };
					}
				}
			}
		}

    	private int GetMemberIdxByFieldNamingStrategies(List<string> candidateMembersNames, Property property)
    	{
    		int exactFieldIdx = -1;
    		foreach (var ns in DefaultFieldNamningStrategies)
    		{
    			var fieldName = ns.GetFieldName(property.Name);
    			exactFieldIdx = candidateMembersNames.IndexOf(fieldName);
    			if (exactFieldIdx >= 0)
    			{
    				break;
    			}
    		}
    		return exactFieldIdx;
    	}

		private void AddPropertiesFromClass(System.Type clazz)
		{
			//No need to go to base class, the .NET GetProperty method can bring the base properties also
			//System.Type superclazz = clazz.BaseType;
			//if (!"System.Object".Equals(superclazz.FullName))
			//{
			//    AddPropertiesFromClass(superclazz);
			//}

			foreach (var declaredPersistentProperty in GetPersistentInfo(clazz, _persistentPropertiesSource.PropertyEnumerator))
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
					new AuditedPropertiesReader(ModificationStore.FULL, componentPropertiesSource, componentData,
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
	        if (_globalCfg.isDoNotAuditOptimisticLockingField()) 
            {
	            //Version jpaVer = property.getAnnotation(typeof(Version));
	            var jpaVer = (VersionAttribute)Attribute.GetCustomAttribute(property, typeof(VersionAttribute));
	            if (jpaVer != null) {
	                return false;
	            }
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

	    private static AuditJoinTableAttribute DEFAULT_AUDIT_JOIN_TABLE = new DefaultAuditJoinTableAttribute();

        private class ComponentPropertiesSource : IPersistentPropertiesSource {
		    private System.Type xclass;
		    private Component component;

		    public ComponentPropertiesSource(Component component) {
			    try {                    
                    this.xclass = component.ComponentClass;

				    //this.xclass = reflectionManager.classForName(component.getComponentClassName(), this.getClass());
			    } catch (Exception e) {
				    throw new MappingException(e);
			    }

			    this.component = component;
		    }

		    public IEnumerable<Property> PropertyEnumerator { get { return component.PropertyIterator; } }
		    public Property GetProperty(String propertyName) { return component.GetProperty(propertyName); }
		    public System.Type GetClass() { return xclass; }
	    }
    }
}

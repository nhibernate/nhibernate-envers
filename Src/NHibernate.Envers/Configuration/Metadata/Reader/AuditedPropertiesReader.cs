using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	/// <summary>
	/// Reads persistent properties form a
	/// <see cref="IPersistentPropertiesSource"/>
	/// and adds the ones that are audited to a
	/// <see cref="IAuditedPropertiesHolder"/>
	/// filling all the auditing data.
	/// </summary>
	public class AuditedPropertiesReader
	{
		private readonly IMetaDataStore _metaDataStore;
		private readonly IPersistentPropertiesSource _persistentPropertiesSource;
		private readonly IAuditedPropertiesHolder _auditedPropertiesHolder;
		private readonly GlobalConfiguration _globalCfg;
		private readonly string _propertyNamePrefix;
		private readonly ISet<string> _overriddenAuditedProperties;
		private readonly ISet<string> _overriddenNotAuditedProperties;
		private readonly ISet<System.Type> _overriddenAuditedClasses;
		private readonly ISet<System.Type> _overriddenNotAuditedClasses;

		public AuditedPropertiesReader(IMetaDataStore metaDataStore,
										IPersistentPropertiesSource persistentPropertiesSource,
										IAuditedPropertiesHolder auditedPropertiesHolder,
										GlobalConfiguration globalCfg,
										string propertyNamePrefix)
		{
			_metaDataStore = metaDataStore;
			_persistentPropertiesSource = persistentPropertiesSource;
			_auditedPropertiesHolder = auditedPropertiesHolder;
			_globalCfg = globalCfg;
			_propertyNamePrefix = propertyNamePrefix;
			_overriddenAuditedProperties = new HashSet<string>();
			_overriddenNotAuditedProperties = new HashSet<string>();
			_overriddenAuditedClasses = new HashSet<System.Type>();
			_overriddenNotAuditedClasses = new HashSet<System.Type>();
		}

		public void Read()
		{
			// Retrieve classes and properties that are explicitly marked for auditing process by any superclass
			// of currently mapped entity or itself.
			var clazz = _persistentPropertiesSource.Class;
			readAuditOverrides(clazz);
			addPropertiesFromClass(clazz);
		}

		/// <summary>
		/// Recursively constructs sets of audited and not audited properties and classes which behavior has been overridden
		/// using <see cref="AuditOverrideAttribute"/>.
		/// </summary>
		/// <param name="clazz">Class that is being processed. Currently mapped entity shall be passed during first invocation.</param>
		private void readAuditOverrides(System.Type clazz)
		{
			var auditOverrides = computeAuditOverrides(clazz);
			foreach (var auditOverrideAttribute in auditOverrides)
			{
				var propertyName = auditOverrideAttribute.PropertyName;
				var overrideClass = auditOverrideAttribute.ForClass;
				if (propertyName != null)
				{
					if(overrideClass!= null)
						throw new MappingException("Both PropertyName and ForClass is set on " + _persistentPropertiesSource.Class.FullName + ". This is not allowed.");
					// Overridden @Audited annotation on property level.
					var property = getProperty(propertyName);
					if (auditOverrideAttribute.IsAudited)
					{
						// If the property has not been marked as not audited by the subclass.
						if(!_overriddenNotAuditedProperties.Contains(property))
							_overriddenAuditedProperties.Add(property);
					}
					else
					{
						// If the property has not been marked as audited by the subclass.
						if(!_overriddenAuditedProperties.Contains(property))
							_overriddenNotAuditedProperties.Add(property);
					}
				}
				else
				{
					if (overrideClass != null)
					{
						checkSuperclass(clazz, overrideClass);
						// Overridden AuditedAttribute on class level.
						if (auditOverrideAttribute.IsAudited)
						{
							if (!_overriddenNotAuditedClasses.Contains(overrideClass))
								_overriddenAuditedClasses.Add(overrideClass);
						}
						else
						{
							if (!_overriddenAuditedClasses.Contains(overrideClass))
								_overriddenNotAuditedClasses.Add(overrideClass);
						}
					}
				}
			}
			var superClass = clazz.BaseType;
			if(!clazz.IsInterface && superClass != typeof(object))
				readAuditOverrides(superClass);
		}

		private string getProperty(string propertyName)
		{
			foreach (var persistentProperty in _persistentPropertiesSource.DeclaredPersistentProperties
										.Where(persistentProperty => persistentProperty.Property.Name.Equals(propertyName)))
			{
				return persistentProperty.Property.Name;
			}

			throw new MappingException("Property '" + propertyName + "' not found in class " + _persistentPropertiesSource.Class.FullName + ".");
		}

		private IEnumerable<AuditOverrideAttribute> computeAuditOverrides(System.Type clazz)
		{
			IEntityMeta entityMeta;
			return _metaDataStore.EntityMetas.TryGetValue(clazz, out entityMeta) ? 
				entityMeta.ClassMetas.OfType<AuditOverrideAttribute>().ToList() : 
				Enumerable.Empty<AuditOverrideAttribute>();
		}

		/// <summary>
		/// Checks whether one class is assignable from another. If not <see cref="MappingException"/> is thrown.
		/// </summary>
		/// <param name="child">Subclass.</param>
		/// <param name="parent">Superclass.</param>
		private static void checkSuperclass(System.Type child, System.Type parent)
		{
			if (!parent.IsAssignableFrom(child))
				throw new MappingException("Class " + parent.FullName + " is not assignable from " + child.FullName + ". " +
										"Please revise Envers configuration for " + child.FullName + " type.");
		}

		/// <summary>
		/// </summary>
		/// <param name="clazz">Class which properties are currently being added.</param>
		/// <returns>
		/// <see cref="AuditedAttribute"/> of specified class. If processed type hasn't been explicitly marked, method
		/// checks whether given class exists in <see cref="_overriddenAuditedClasses"/> collection.
		/// In case of success, <see cref="AuditedAttribute"/> of currently mapped entity is returned, otherwise
		/// <code>null</code>. If processed type exists in <see cref="_overriddenNotAuditedClasses"/> collection,
		/// the result is also <code>null</code>.
		/// </returns>
		private AuditedAttribute computeAuditConfiguration(System.Type clazz)
		{
			var allClassAudited = _metaDataStore.ClassMeta<AuditedAttribute>(clazz);
			// If processed class is not explicitly marked with @Audited annotation, check whether auditing is
			// forced by any of its child entities configuration (@Audited.auditParents).
			if (allClassAudited == null && _overriddenAuditedClasses.Contains(clazz))
			{
				// Declared audited parent copies @Audited.modStore and @Audited.targetAuditMode configuration from
				// currently mapped entity.
				allClassAudited = _metaDataStore.ClassMeta<AuditedAttribute>(_persistentPropertiesSource.Class) ??
										new AuditedAttribute();
			}
			else if (allClassAudited != null && _overriddenNotAuditedClasses.Contains(clazz))
				return null;
			return allClassAudited;
		}

		/// <summary>
		/// Recursively adds all audited properties of entity class and its superclasses.
		/// </summary>
		/// <param name="clazz">Currently processed class.</param>
		private void addPropertiesFromClass(System.Type clazz)
		{
			var allClassAudited = computeAuditConfiguration(clazz);
			addFromProperties(allClassAudited, clazz);
			if (allClassAudited != null || !_auditedPropertiesHolder.IsEmpty())
			{
				var superclazz = clazz.BaseType;
				if (!clazz.IsInterface && typeof(object) != superclazz)
				{
					addPropertiesFromClass(superclazz);
				}
			}
		}

		private void addFromProperties(AuditedAttribute allClassAudited, System.Type currentClass)
		{
			foreach (var declaredPersistentProperty in _persistentPropertiesSource.DeclaredPersistentProperties)
			{
				// If the property was already defined by the subclass, is ignored by superclasses
				if (_auditedPropertiesHolder.Contains(declaredPersistentProperty.Property.Name))
					continue;

				//only get the property on specific class if not a component
				if ((declaredPersistentProperty.Member.DeclaringType != currentClass && !_persistentPropertiesSource.IsComponent) &&
					!declaredPersistentProperty.Member.Equals(DeclaredPersistentProperty.NotAvailableMemberInfo) && 
					!_overriddenAuditedProperties.Contains(declaredPersistentProperty.Property.Name))
					continue;

				var propertyValue = declaredPersistentProperty.Property.Value;

				var componentValue = propertyValue as Component;
				if (componentValue != null)
				{
					if (declaredPersistentProperty.Member.Equals(DeclaredPersistentProperty.NotAvailableMemberInfo))
					{
						addFromPropertiesGroup(declaredPersistentProperty, componentValue, allClassAudited);
					}
					else
					{
						addFromComponentProperty(declaredPersistentProperty, componentValue, allClassAudited);
					}
				}
				else
				{
					addFromNotComponentProperty(declaredPersistentProperty, allClassAudited);
				}
			}
		}

		private void addFromPropertiesGroup(DeclaredPersistentProperty property, Component componentValue, AuditedAttribute allClassAudited)
		{
			var componentData = new ComponentAuditingData();
			var isAudited = fillPropertyData(property.Member,
											property.Property.Name,
											componentData,
											property.Property.PropertyAccessorName,
											allClassAudited);
			if (isAudited)
			{
				componentData.BeanName = null;
				componentData.Name = property.Property.Name;
				var componentPropertiesSource = new ComponentPropertiesSource(componentValue);
				var audPropReader = new AuditedPropertiesReader(_metaDataStore,
											componentPropertiesSource, componentData,
											_globalCfg,
											_propertyNamePrefix +
											MappingTools.CreateComponentPrefix(property.Property.Name));
				audPropReader.Read();

				_auditedPropertiesHolder.AddPropertyAuditingData(property.Property.Name, componentData);
			}
		}

		private void addFromComponentProperty(DeclaredPersistentProperty property, Component componentValue, AuditedAttribute allClassAudited)
		{
			var componentData = new ComponentAuditingData();
			var isAudited = fillPropertyData(property.Member,
											property.Property.Name,
											componentData,
											property.Property.PropertyAccessorName,
											allClassAudited);

			var componentPropertiesSource = new ComponentPropertiesSource(componentValue);
			var audPropReader = new ComponentAuditedPropertiesReader(_metaDataStore,
										componentPropertiesSource, componentData,
										_globalCfg,
										_propertyNamePrefix +
										MappingTools.CreateComponentPrefix(property.Property.Name));
			audPropReader.Read();

			if (isAudited)
			{
				// Now we know that the property is audited
				_auditedPropertiesHolder.AddPropertyAuditingData(property.Property.Name, componentData);
			}
		}

		private void addFromNotComponentProperty(DeclaredPersistentProperty property, AuditedAttribute allClassAudited)
		{
			var propertyData = new PropertyAuditingData();
			var isAudited = fillPropertyData(property.Member,
													property.Property.Name,
													propertyData,
													property.Property.PropertyAccessorName,
													allClassAudited);
			if (isAudited)
			{
				// Now we know that the property is audited
				_auditedPropertiesHolder.AddPropertyAuditingData(property.Property.Name, propertyData);
			}
		}

		/// <summary>
		/// Checks if a property is audited and if yes, fills all of its data.
		/// </summary>
		/// <param name="property">Property to check.</param>
		/// <param name="mappedPropertyName">NH Property name</param>
		/// <param name="propertyData">Property data, on which to set this property's modification store.</param>
		/// <param name="accessType">Access type for the property.</param>
		/// <param name="allClassAudited">Is class fully audited</param>
		/// <returns>False if this property is not audited.</returns>
		private bool fillPropertyData(MemberInfo property,
										string mappedPropertyName,
										PropertyAuditingData propertyData,
										string accessType,
										AuditedAttribute allClassAudited)
		{

			// check if a property is declared as not audited to exclude it
			// useful if a class is audited but some properties should be excluded
			if ((_metaDataStore.MemberMeta<NotAuditedAttribute>(property) != null && !_overriddenAuditedProperties.Contains(mappedPropertyName)) ||
					_overriddenNotAuditedProperties.Contains(mappedPropertyName))
			{
				return false;
			}
			// if the optimistic locking field has to be unversioned and the current property
			// is the optimistic locking field, don't audit it
			if (_globalCfg.DoNotAuditOptimisticLockingField &&
				_persistentPropertiesSource.VersionedProperty != null &&
				_persistentPropertiesSource.VersionedProperty.Name.Equals(mappedPropertyName))
			{
				return false;
			}

			if (!CheckAudited(property, mappedPropertyName, propertyData, allClassAudited))
			{
				return false;
			}

			var propertyName = _propertyNamePrefix + mappedPropertyName;
			propertyData.Name = propertyName;
			propertyData.ModifiedFlagName = MetadataTools.ModifiedFlagPropertyName(propertyName, _globalCfg.ModifiedFlagSuffix);
			propertyData.BeanName = mappedPropertyName;
			propertyData.AccessType = accessType;

			addPropertyJoinTables(property, propertyData);
			addPropertyAuditingOverrides(property, propertyData);
			if (!processPropertyAuditingOverrides(property, propertyData))
			{
				return false; // not audited due to AuditOverride annotation
			}
			setPropertyAuditMappedBy(property, propertyData);
			setCustomMapper(property, propertyData);

			return true;
		}

		protected virtual bool CheckAudited(MemberInfo property, string mappedPropertyName, PropertyAuditingData propertyData, AuditedAttribute allClassAudited)
		{
			// Checking if this property is explicitly audited or if all properties are.
			var aud = _metaDataStore.MemberMeta<AuditedAttribute>(property) ?? allClassAudited;
			if (aud == null && _overriddenAuditedProperties.Contains(mappedPropertyName) && !_overriddenNotAuditedProperties.Contains(mappedPropertyName))
			{
				// Assigning AuditedAttribute defaults. If anyone needs to customize those values in the future,
				// appropriate fields shall be added to AuditOverrideAttribute annotation.
				aud=new AuditedAttribute();
			}
			if (aud != null)
			{
				propertyData.RelationTargetAuditMode = aud.TargetAuditMode;
				propertyData.UsingModifiedFlag = checkUsingModifiedFlag(aud);
				return true;
			}
			return false;
		}

		private bool checkUsingModifiedFlag(AuditedAttribute aud)
		{
			return _globalCfg.IsGlobalWithModifiedFlag || aud.WithModifiedFlag;
		}

		private void setPropertyAuditMappedBy(MemberInfo property, PropertyAuditingData propertyData)
		{
			var auditMappedBy = _metaDataStore.MemberMeta<AuditMappedByAttribute>(property);
			if (auditMappedBy != null)
			{
				propertyData.MappedBy = auditMappedBy.MappedBy;
				propertyData.ForceInsertable = auditMappedBy.ForceInsertOverride;
				propertyData.PositionMappedBy = auditMappedBy.PositionMappedBy;
			}
		}

		private void setCustomMapper(MemberInfo property, PropertyAuditingData propertyData)
		{
			var customMapper = _metaDataStore.MemberMeta<CustomCollectionMapperAttribute>(property);
			if (customMapper != null)
			{
				propertyData.CustomCollectionMapperFactory = (ICustomCollectionMapperFactory) Activator.CreateInstance(customMapper.CustomCollectionFactory);
			}
		}

		private void addPropertyJoinTables(MemberInfo property, PropertyAuditingData propertyData)
		{
			// first set the join table based on the AuditJoinTable annotation
			var joinTable = _metaDataStore.MemberMeta<AuditJoinTableAttribute>(property);
			propertyData.JoinTable = joinTable ?? DEFAULT_AUDIT_JOIN_TABLE;
		}

		/***
		 * Add the {@link org.hibernate.envers.AuditOverride} annotations.
		 *
		 * @param property the property being processed
		 * @param propertyData the Envers auditing data for this property
		 */
		private void addPropertyAuditingOverrides(MemberInfo property, PropertyAuditingData propertyData)
		{
			var annotationOverride = _metaDataStore.MemberMeta<AuditOverrideAttribute>(property);
			if (annotationOverride != null)
			{
				propertyData.AddAuditingOverride(annotationOverride);
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
		private bool processPropertyAuditingOverrides(MemberInfo property, PropertyAuditingData propertyData)
		{
			var audPropHolderAsComponentAudData = _auditedPropertiesHolder as ComponentAuditingData;
			// if this property is part of a component, process all override annotations
			if (audPropHolderAsComponentAudData != null)
			{
				var overrides = audPropHolderAsComponentAudData.AuditingOverrides;
				foreach (var ovr in overrides)
				{
					if (property.Name.Equals(ovr.PropertyName))
					{
						// the override applies to this property
						if (!ovr.IsAudited)
						{
							return false;
						}
						propertyData.JoinTable = ovr;
					}
				}
			}
			return true;
		}

		private static readonly AuditJoinTableAttribute DEFAULT_AUDIT_JOIN_TABLE = new AuditJoinTableAttribute();

		internal class ComponentPropertiesSource : IPersistentPropertiesSource
		{
			public ComponentPropertiesSource(Component component)
			{
				var xclass = component.IsDynamic ?
					typeof(markerClassForDynamicEntity) :
					component.ComponentClass;
				DeclaredPersistentProperties = component.IsDynamic ?
					createDeclaredPersistentPropertyForDynamicComponent(component) :
					PropertyAndMemberInfo.PersistentInfo(xclass, component.PropertyIterator);
				Class = xclass;
			}

			private static IEnumerable<DeclaredPersistentProperty> createDeclaredPersistentPropertyForDynamicComponent(Component component)
			{
				return component.PropertyIterator
					.Select(property => new DeclaredPersistentProperty(property, DeclaredPersistentProperty.NotAvailableMemberInfo)).ToList();
			}

			public IEnumerable<DeclaredPersistentProperty> DeclaredPersistentProperties { get; private set; }
			public System.Type Class { get; private set; }

			public bool IsComponent
			{
				get { return true; }
			}

			public Property VersionedProperty
			{
				get { return null; }
			}
		}

		private class markerClassForDynamicEntity
		{
		}
	}
}

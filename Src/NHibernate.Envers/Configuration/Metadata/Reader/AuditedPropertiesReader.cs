using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
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
		private readonly ModificationStore _defaultStore;
		private readonly IPersistentPropertiesSource _persistentPropertiesSource;
		private readonly IAuditedPropertiesHolder _auditedPropertiesHolder;
		private readonly GlobalConfiguration _globalCfg;
		private readonly string _propertyNamePrefix;

		public AuditedPropertiesReader(IMetaDataStore metaDataStore,
										ModificationStore defaultStore,
										IPersistentPropertiesSource persistentPropertiesSource,
										IAuditedPropertiesHolder auditedPropertiesHolder,
										GlobalConfiguration globalCfg,
										string propertyNamePrefix)
		{
			_metaDataStore = metaDataStore;
			_defaultStore = defaultStore;
			_persistentPropertiesSource = persistentPropertiesSource;
			_auditedPropertiesHolder = auditedPropertiesHolder;
			_globalCfg = globalCfg;
			_propertyNamePrefix = propertyNamePrefix;
		}

		public void Read()
		{
			addPropertiesFromClass();
		}

		private void addPropertiesFromClass()
		{
			foreach (var declaredPersistentProperty in _persistentPropertiesSource.DeclaredPersistentProperties)
			{
				var propertyValue = declaredPersistentProperty.Property.Value;

				PropertyAuditingData propertyData;
				bool isAudited;
				var componentValue = propertyValue as Component;
				if (componentValue != null)
				{
					var componentData = new ComponentAuditingData();
					isAudited = FillPropertyData(declaredPersistentProperty.Member,
													declaredPersistentProperty.Property.Name,
													componentData,
													declaredPersistentProperty.Property.PropertyAccessorName);

					IPersistentPropertiesSource componentPropertiesSource = new ComponentPropertiesSource(componentValue);
					new AuditedPropertiesReader(_metaDataStore,
												ModificationStore.Full, componentPropertiesSource, componentData,
												_globalCfg,
												_propertyNamePrefix +
												MappingTools.CreateComponentPrefix(declaredPersistentProperty.Property.Name))
						.Read();

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

		/// <summary>
		/// Checks if a property is audited and if yes, fills all of its data.
		/// </summary>
		/// <param name="property">Property to check.</param>
		/// <param name="mappedPropertyName">NH Property name</param>
		/// <param name="propertyData">Property data, on which to set this property's modification store.</param>
		/// <param name="accessType">Access type for the property.</param>
		/// <returns>False if this property is not audited.</returns>
		private bool FillPropertyData(MemberInfo property,
										string mappedPropertyName,
										PropertyAuditingData propertyData,
										string accessType)
		{

			// check if a property is declared as not audited to exclude it
			// useful if a class is audited but some properties should be excluded
			if (_metaDataStore.MemberMeta<NotAuditedAttribute>(property) != null)
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

			// Checking if this property is explicitly audited or if all properties are.
			var aud = _metaDataStore.MemberMeta<AuditedAttribute>(property);
			if (aud != null)
			{
				propertyData.Store = aud.ModStore;
				propertyData.RelationTargetAuditMode = aud.TargetAuditMode;
			}
			else
			{
				if (_defaultStore != ModificationStore.None)
				{
					propertyData.Store = _defaultStore;
				}
				else
				{
					return false;
				}
			}

			propertyData.Name = _propertyNamePrefix + mappedPropertyName;
			propertyData.BeanName = mappedPropertyName;
			propertyData.AccessType = accessType;

			AddPropertyJoinTables(property, propertyData);
			AddPropertyAuditingOverrides(property, propertyData);
			if (!ProcessPropertyAuditingOverrides(property, propertyData))
			{
				return false; // not audited due to AuditOverride annotation
			}
			SetPropertyAuditMappedBy(property, propertyData);

			return true;
		}

		private void SetPropertyAuditMappedBy(MemberInfo property, PropertyAuditingData propertyData)
		{

			var auditMappedBy = _metaDataStore.MemberMeta<AuditMappedByAttribute>(property);
			if (auditMappedBy != null)
			{
				propertyData.AuditMappedBy = auditMappedBy.MappedBy;
				if (!string.IsNullOrEmpty(auditMappedBy.PositionMappedBy))
				{
					propertyData.PositionMappedBy = auditMappedBy.PositionMappedBy;
				}
			}
		}

		private void AddPropertyJoinTables(MemberInfo property, PropertyAuditingData propertyData)
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
		private void AddPropertyAuditingOverrides(MemberInfo property, PropertyAuditingData propertyData)
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
		private bool ProcessPropertyAuditingOverrides(MemberInfo property, PropertyAuditingData propertyData)
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

		private class ComponentPropertiesSource : IPersistentPropertiesSource
		{
			private readonly System.Type xclass;
			private readonly IEnumerable<DeclaredPersistentProperty> _declaredPersistentProperties;

			public ComponentPropertiesSource(Component component)
			{
				xclass = component.ComponentClass;
				_declaredPersistentProperties = component.IsDynamic ?
					createDeclaredPersistentPropertyForDynamicComponent(component) : PropertyAndMemberInfo.PersistentInfo(xclass, component.PropertyIterator);
			}

			private static IEnumerable<DeclaredPersistentProperty> createDeclaredPersistentPropertyForDynamicComponent(Component component)
			{
				//todo: rk - using system.object.ToString here for now - just to get no "hit" when looking for attributes... 
				//Fix this later. Probably by allowing asking for attributes on methodinfo "null" instead...
				return component.PropertyIterator
					.Select(property => new DeclaredPersistentProperty { Property = property, Member = typeof(object).GetMethod("ToString") }).ToList();
			}

			public IEnumerable<DeclaredPersistentProperty> DeclaredPersistentProperties
			{
				get { return _declaredPersistentProperties; }
			}

			public Property VersionedProperty
			{
				get { return null; }
			}
		}
	}
}

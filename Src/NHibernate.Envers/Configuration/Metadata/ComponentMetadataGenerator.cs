using System;
using NHibernate.Mapping;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities.Mapper;
using System.Xml;

namespace NHibernate.Envers.Configuration.Metadata
{
	/// <summary>
	/// Generates metadata for components.
	/// </summary>
	public sealed class ComponentMetadataGenerator 
	{
		private readonly AuditMetadataGenerator mainGenerator;

		public ComponentMetadataGenerator(AuditMetadataGenerator auditMetadataGenerator) 
		{
			mainGenerator = auditMetadataGenerator;
		}

		public void AddComponent(XmlElement parent, PropertyAuditingData propertyAuditingData,
								 IValue value, ICompositeMapperBuilder mapper, String entityName,
								 EntityXmlMappingData xmlMappingData, bool firstPass) 
		{
			var prop_component = (Component) value;

			var componentMapper = mapper.AddComponent(propertyAuditingData.GetPropertyData(),
					prop_component.ComponentClassName);

			// The property auditing data must be for a component.
			var componentAuditingData = (ComponentAuditingData) propertyAuditingData;

			// Adding all properties of the component
			foreach (var property in prop_component.PropertyIterator)
			{
				var componentPropertyAuditingData = componentAuditingData.GetPropertyAuditingData(property.Name);

				// Checking if that property is audited
				if (componentPropertyAuditingData != null)
				{
					mainGenerator.AddValue(parent, property.Value, componentMapper, entityName, xmlMappingData,
							componentPropertyAuditingData, property.IsInsertable, firstPass);
				}
			}
		}
	}
}

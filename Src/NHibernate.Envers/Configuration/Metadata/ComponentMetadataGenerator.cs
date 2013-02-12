using System.Xml.Linq;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Mapping;

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

		public void AddComponent(XElement parent, PropertyAuditingData propertyAuditingData,
								 IValue value, ICompositeMapperBuilder mapper, string entityName,
								 EntityXmlMappingData xmlMappingData, bool firstPass, bool insertable) 
		{
			var propComponent = (Component) value;

			var componentMapper = mapper.AddComponent(propertyAuditingData.GetPropertyData(),
			                                                              propComponent.ComponentClassName);				

			// The property auditing data must be for a component.
			var componentAuditingData = (ComponentAuditingData) propertyAuditingData;

			// Adding all properties of the component
			foreach (var property in propComponent.PropertyIterator)
			{
				var componentPropertyAuditingData = componentAuditingData.GetPropertyAuditingData(property.Name);

				// Checking if that property is audited
				if (componentPropertyAuditingData != null)
				{
					mainGenerator.AddValue(parent, property.Value, componentMapper, entityName, xmlMappingData,
							componentPropertyAuditingData, property.IsInsertable && insertable, firstPass, false);
				}
			}
		}
	}
}

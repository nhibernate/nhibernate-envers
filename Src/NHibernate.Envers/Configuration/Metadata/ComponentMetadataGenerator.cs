using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities.Mapper;
using System.Xml;

namespace NHibernate.Envers.Configuration.Metadata
{
    /**
     * Generates metadata for components.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public sealed class ComponentMetadataGenerator {
	    private AuditMetadataGenerator mainGenerator;

	    public ComponentMetadataGenerator(AuditMetadataGenerator auditMetadataGenerator) {
		    mainGenerator = auditMetadataGenerator;
	    }

	    //@SuppressWarnings({"unchecked"})
	    public void AddComponent(XmlElement parent, PropertyAuditingData propertyAuditingData,
							     IValue value, ICompositeMapperBuilder mapper, String entityName,
							     EntityXmlMappingData xmlMappingData, bool firstPass) {
		    Component prop_component = (Component) value;

		    ICompositeMapperBuilder componentMapper = mapper.AddComponent(propertyAuditingData.GetPropertyData(),
				    prop_component.ComponentClassName);

		    // The property auditing data must be for a component.
		    ComponentAuditingData componentAuditingData = (ComponentAuditingData) propertyAuditingData;

		    // Adding all properties of the component
		    IEnumerator<Property> properties = (IEnumerator<Property>) prop_component.PropertyIterator.GetEnumerator();
		    while (properties.MoveNext()) {
			    Property property = properties.Current;

			    PropertyAuditingData componentPropertyAuditingData =
					    componentAuditingData.GetPropertyAuditingData(property.Name);

			    // Checking if that property is audited
			    if (componentPropertyAuditingData != null) {
				    mainGenerator.AddValue(parent, property.Value, componentMapper, entityName, xmlMappingData,
						    componentPropertyAuditingData, property.IsInsertable, firstPass);
			    }
		    }
	    }
    }
}

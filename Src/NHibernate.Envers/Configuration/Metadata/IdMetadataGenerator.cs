using System.Collections.Generic;
using System.Xml;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Mapping;
using NHibernate.Type;

namespace NHibernate.Envers.Configuration.Metadata
{
	public sealed class IdMetadataGenerator 
	{
		private readonly AuditMetadataGenerator mainGenerator;

		public IdMetadataGenerator(AuditMetadataGenerator auditMetadataGenerator) 
		{
			mainGenerator = auditMetadataGenerator;
		}
	
		private void AddIdProperties(XmlElement parent, IEnumerable<Property> properties, ISimpleMapperBuilder mapper, bool key) 
		{
			foreach (var property in properties)
			{
				var propertyType = property.Type;
				if (!"_identifierMapper".Equals(property.Name))
				{
					if (propertyType is ImmutableType)
					{
						// Last but one parameter: ids are always insertable
						mainGenerator.BasicMetadataGenerator.AddBasic(parent,
								GetIdPersistentPropertyAuditingData(property),
								property.Value, mapper, true, key);
					}
					else
					{
						throw new MappingException("Type not supported: " + propertyType.Name);
					}
				}
			}
		}

		public IdMappingData AddId(PersistentClass pc) 
		{
			// Xml mapping which will be used for relations
			var idMappingDoc = new XmlDocument();
			var relIdMapping = idMappingDoc.CreateElement("properties"); 
			// Xml mapping which will be used for the primary key of the versions table
			var origIdMapping = idMappingDoc.CreateElement("composite-id"); 

			var idProp = pc.IdentifierProperty;
			var idMapper = pc.IdentifierMapper;

			// Checking if the id mapping is supported
			if (idMapper == null && idProp == null) 
			{
				return null;
			}

			ISimpleIdMapperBuilder mapper;
			if (idMapper != null) 
			{
				// Multiple id
				throw new MappingException("Multi id mapping isn't (wasn't?) available in NH Core");
			}
			if (idProp.IsComposite) 
			{
				// Embedded id
				var idComponent = (Component) idProp.Value;

				mapper = new EmbeddedIdMapper(GetIdPropertyData(idProp), idComponent.ComponentClass);
				AddIdProperties(relIdMapping, idComponent.PropertyIterator, mapper, false);

				// null mapper - the mapping where already added the first time, now we only want to generate the xml
				AddIdProperties(origIdMapping, idComponent.PropertyIterator, null, true);
			} 
			else 
			{
				// Single id
				mapper = new SingleIdMapper();

				// Last but one parameter: ids are always insertable
				mainGenerator.BasicMetadataGenerator.AddBasic(relIdMapping,
				                                              GetIdPersistentPropertyAuditingData(idProp),
				                                              idProp.Value, mapper, true, false);

				// null mapper - the mapping where already added the first time, now we only want to generate the xml
				mainGenerator.BasicMetadataGenerator.AddBasic(origIdMapping,
				                                              GetIdPersistentPropertyAuditingData(idProp),
				                                              idProp.Value, null, true, true);
			}

			origIdMapping.SetAttribute("name", mainGenerator.VerEntCfg.OriginalIdPropName);

			// Adding a relation to the revision entity (effectively: the "revision number" property)
			mainGenerator.AddRevisionInfoRelation(origIdMapping);

			return new IdMappingData(mapper, origIdMapping, relIdMapping);
		}

		private static PropertyData GetIdPropertyData(Property property) 
		{
			return new PropertyData(property.Name, property.Name, property.PropertyAccessorName,
					ModificationStore.Full);
		}

		private static PropertyAuditingData GetIdPersistentPropertyAuditingData(Property property) 
		{
			return new PropertyAuditingData(property.Name, property.PropertyAccessorName,
					ModificationStore.Full, RelationTargetAuditMode.Audited, null, null, false);
		}
	}
}

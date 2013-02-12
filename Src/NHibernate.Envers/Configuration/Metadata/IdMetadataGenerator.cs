using System.Collections.Generic;
using System.Xml.Linq;
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
		private readonly AuditMetadataGenerator _mainGenerator;

		public IdMetadataGenerator(AuditMetadataGenerator auditMetadataGenerator) 
		{
			_mainGenerator = auditMetadataGenerator;
		}
	
		private void addIdProperties(XElement parent, IEnumerable<Property> properties, ISimpleMapperBuilder mapper, bool key) 
		{
			foreach (var property in properties)
			{
				var propertyType = property.Type;
				if (!"_identifierMapper".Equals(property.Name))
				{
					if (!propertyType.IsMutable)
					{
						if (propertyType is ManyToOneType)
						{
							_mainGenerator.BasicMetadataGenerator.AddKeyManyToOne(parent,
									 getIdPersistentPropertyAuditingData(property),
									 property.Value, mapper);
						}
						else
						{
							// Last but one parameter: ids are always insertable
							_mainGenerator.BasicMetadataGenerator.AddBasic(parent,
									getIdPersistentPropertyAuditingData(property),
									property.Value, mapper, true, key);							
						}
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
			var relIdMapping = new XElement(MetadataTools.CreateElementName("properties"));
			// Xml mapping which will be used for the primary key of the versions table
			var origIdMapping = new XElement(MetadataTools.CreateElementName("composite-id"), 
				new XAttribute("name", _mainGenerator.VerEntCfg.OriginalIdPropName)); 

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

				mapper = new EmbeddedIdMapper(getIdPropertyData(idProp), idComponent.ComponentClass);
				addIdProperties(relIdMapping, idComponent.PropertyIterator, mapper, false);

				// null mapper - the mapping where already added the first time, now we only want to generate the xml
				addIdProperties(origIdMapping, idComponent.PropertyIterator, null, true);
			} 
			else 
			{
				// Single id
				mapper = new SingleIdMapper();

				// Last but one parameter: ids are always insertable
				_mainGenerator.BasicMetadataGenerator.AddBasic(relIdMapping,
				                                              getIdPersistentPropertyAuditingData(idProp),
				                                              idProp.Value, mapper, true, false);

				// null mapper - the mapping where already added the first time, now we only want to generate the xml
				_mainGenerator.BasicMetadataGenerator.AddBasic(origIdMapping,
				                                              getIdPersistentPropertyAuditingData(idProp),
				                                              idProp.Value, null, true, true);
			}

			// Adding a relation to the revision entity (effectively: the "revision number" property)
			_mainGenerator.AddRevisionInfoRelation(origIdMapping);

			return new IdMappingData(mapper, origIdMapping, relIdMapping);
		}

		private static PropertyData getIdPropertyData(Property property) 
		{
			return new PropertyData(property.Name, property.Name, property.PropertyAccessorName);
		}

		private static PropertyAuditingData getIdPersistentPropertyAuditingData(Property property) 
		{
			return new PropertyAuditingData(property.Name, property.PropertyAccessorName);
		}
	}
}

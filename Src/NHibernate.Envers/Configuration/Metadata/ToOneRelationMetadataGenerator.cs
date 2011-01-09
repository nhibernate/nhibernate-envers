using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Tools.Graph;
using NHibernate.Envers.Entities;
using NHibernate.Mapping;
using NHibernate.Properties;
using NHibernate.Type;
using Iesi.Collections.Generic;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Entities.Mapper.Relation;


namespace NHibernate.Envers.Configuration.Metadata

{
/**
 * Generates metadata for to-one relations (reference-valued properties).
 * @author Catalina Panait, port of Envers omonyme class by Adam Warski (adam at warski dot org)
 */
    public sealed class ToOneRelationMetadataGenerator {
        private AuditMetadataGenerator mainGenerator;

        public ToOneRelationMetadataGenerator(AuditMetadataGenerator auditMetadataGenerator) {
            mainGenerator = auditMetadataGenerator;
        }

        //@SuppressWarnings({"unchecked"})
        public void AddToOne(XmlElement parent, PropertyAuditingData propertyAuditingData, IValue value,
                      ICompositeMapperBuilder mapper, String entityName, bool insertable) {
            String referencedEntityName = ((ToOne)value).ReferencedEntityName;

            IdMappingData idMapping = mainGenerator.GetReferencedIdMappingData(entityName, referencedEntityName,
                    propertyAuditingData, true);

            String lastPropertyPrefix = MappingTools.createToOneRelationPrefix(propertyAuditingData.Name);

            // Generating the id mapper for the relation
            IIdMapper relMapper = idMapping.IdMapper.PrefixMappedProperties(lastPropertyPrefix);

            // Storing information about this relation
            mainGenerator.EntitiesConfigurations[entityName].AddToOneRelation(
                    propertyAuditingData.Name, referencedEntityName, relMapper, insertable);

            // If the property isn't insertable, checking if this is not a "fake" bidirectional many-to-one relationship,
            // that is, when the one side owns the relation (and is a collection), and the many side is non insertable.
            // When that's the case and the user specified to store this relation without a middle table (using
            // @AuditMappedBy), we have to make the property insertable for the purposes of Envers. In case of changes to
            // the entity that didn't involve the relation, it's value will then be stored properly. In case of changes
            // to the entity that did involve the relation, it's the responsibility of the collection side to store the
            // proper data.
            bool nonInsertableFake;
            if (!insertable && propertyAuditingData.ForceInsertable) {
                nonInsertableFake = true;
                insertable = true;
            } else {
                nonInsertableFake = false;
            }

            
            // Adding an element to the mapping corresponding to the references entity id's
            // Use OwnerDocument.ImportNode() instead of XmlNode.Clone();
            XmlElement properties = (XmlElement)parent.OwnerDocument.ImportNode(idMapping.XmlRelationMapping,true);
            properties.SetAttribute("name",propertyAuditingData.Name);

            MetadataTools.PrefixNamesInPropertyElement(properties, lastPropertyPrefix,
                    MetadataTools.GetColumnNameEnumerator(value.ColumnIterator), false, insertable);
            parent.AppendChild(properties);


            // Adding mapper for the id
            PropertyData propertyData = propertyAuditingData.getPropertyData();
            mapper.AddComposite(propertyData, new ToOneIdMapper(relMapper,propertyData,referencedEntityName,nonInsertableFake));
        }

        //@SuppressWarnings({"unchecked"})
        public void AddOneToOneNotOwning(PropertyAuditingData propertyAuditingData, IValue value,
                                  ICompositeMapperBuilder mapper, String entityName) {
            OneToOne propertyValue = (OneToOne)value;

            String owningReferencePropertyName = propertyValue.ReferencedPropertyName; // mappedBy
            EntityConfiguration configuration = mainGenerator.EntitiesConfigurations[entityName]; 
            if (configuration == null) {
                throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
            }

            IdMappingData ownedIdMapping = configuration.IdMappingData;

            if (ownedIdMapping == null) {
                throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
            }

            String lastPropertyPrefix = MappingTools.createToOneRelationPrefix(owningReferencePropertyName);
            String referencedEntityName = propertyValue.ReferencedEntityName;

            // Generating the id mapper for the relation
            IIdMapper ownedIdMapper = ownedIdMapping.IdMapper.PrefixMappedProperties(lastPropertyPrefix);

            // Storing information about this relation
            mainGenerator.EntitiesConfigurations[entityName].AddToOneNotOwningRelation(
                    propertyAuditingData.Name, owningReferencePropertyName,
                    referencedEntityName, ownedIdMapper);

            // Adding mapper for the id
            PropertyData propertyData = propertyAuditingData.getPropertyData();
            mapper.AddComposite(propertyData, new OneToOneNotOwningMapper(owningReferencePropertyName,
                    referencedEntityName, propertyData));
        }
    }
}

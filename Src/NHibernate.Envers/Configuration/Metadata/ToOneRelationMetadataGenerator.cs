﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Exceptions;
using NHibernate.Mapping;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Entities.Mapper.Relation;


namespace NHibernate.Envers.Configuration.Metadata
{
	/// <summary>
	/// Generates metadata for to-one relations (reference-valued properties).
	/// </summary>
	public sealed class ToOneRelationMetadataGenerator 
	{
		private readonly AuditMetadataGenerator mainGenerator;

		public ToOneRelationMetadataGenerator(AuditMetadataGenerator auditMetadataGenerator) 
		{
			mainGenerator = auditMetadataGenerator;
		}

		public void AddToOne(XmlElement parent, PropertyAuditingData propertyAuditingData, IValue value,
					  ICompositeMapperBuilder mapper, string entityName, bool insertable, IEnumerable<string> fixedColumnNames) 
		{
			var referencedEntityName = ((ToOne)value).ReferencedEntityName;
			var idMapping = mainGenerator.GetReferencedIdMappingData(entityName, referencedEntityName,
					propertyAuditingData, true);

			var lastPropertyPrefix = MappingTools.CreateToOneRelationPrefix(propertyAuditingData.Name);

			// Generating the id mapper for the relation
			var relMapper = idMapping.IdMapper.PrefixMappedProperties(lastPropertyPrefix);

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
			if (!insertable && propertyAuditingData.ForceInsertable) 
			{
				nonInsertableFake = true;
				insertable = true;
			} 
			else 
			{
				nonInsertableFake = false;
			}

			
			// Adding an element to the mapping corresponding to the references entity id's
			// Use OwnerDocument.ImportNode() instead of XmlNode.Clone();
			var properties = (XmlElement)parent.OwnerDocument.ImportNode(idMapping.XmlRelationMapping,true);
			properties.SetAttribute("name",propertyAuditingData.Name);

			if (fixedColumnNames == null)
			{
				MetadataTools.PrefixNamesInPropertyElement(properties, lastPropertyPrefix, MetadataTools.GetColumnNameEnumerator(value.ColumnIterator), false, insertable);				
			}
			else
			{
				MetadataTools.PrefixNamesInPropertyElement(properties, lastPropertyPrefix, fixedColumnNames.GetEnumerator(), false, insertable);
			}
			parent.AppendChild(properties);


			// Adding mapper for the id
			var propertyData = propertyAuditingData.GetPropertyData();
			mapper.AddComposite(propertyData, new ToOneIdMapper(relMapper,propertyData,referencedEntityName,nonInsertableFake));
		}

		public void AddOneToOneNotOwning(PropertyAuditingData propertyAuditingData, OneToOne value, ICompositeMapperBuilder mapper, string entityName) 
		{
			var owningReferencePropertyName = referencePropertyName(value, entityName);

			var configuration = mainGenerator.EntitiesConfigurations[entityName]; 
			if (configuration == null) 
			{
				throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
			}

			var ownedIdMapping = configuration.IdMappingData;

			if (ownedIdMapping == null) 
			{
				throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
			}

			var lastPropertyPrefix = MappingTools.CreateToOneRelationPrefix(owningReferencePropertyName);
			var referencedEntityName = value.ReferencedEntityName;

			// Generating the id mapper for the relation
			var ownedIdMapper = ownedIdMapping.IdMapper.PrefixMappedProperties(lastPropertyPrefix);

			// Storing information about this relation
			mainGenerator.EntitiesConfigurations[entityName].AddToOneNotOwningRelation(
					propertyAuditingData.Name, owningReferencePropertyName,
					referencedEntityName, ownedIdMapper);

			// Adding mapper for the id
			var propertyData = propertyAuditingData.GetPropertyData();
			mapper.AddComposite(propertyData, new OneToOneNotOwningMapper(owningReferencePropertyName,
					referencedEntityName, propertyData));
		}

		private string referencePropertyName(OneToOne value, string entityName)
		{
			var owningReferencePropertyName = value.ReferencedPropertyName;

			if (owningReferencePropertyName == null) //onetoone pk
			{
				foreach (var refProperty in mainGenerator.Cfg.GetClassMapping(value.ReferencedEntityName).PropertyIterator)
				{
					if (refProperty.Value is OneToOne && refProperty.Type.Name.Equals(entityName))
					{
						owningReferencePropertyName = refProperty.Name;
						break;
					}
				}
			}
			if(owningReferencePropertyName==null)
				throw new AuditException("The onetoone mapping on entity " + entityName + ", property " + value.PropertyName + " is not supported!");
			return owningReferencePropertyName;
		}
	}
}
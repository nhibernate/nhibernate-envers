using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NHibernate.Envers.Entities;
using NHibernate.Mapping;
using NHibernate.Envers.Configuration.Metadata.Reader;
using log4net;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Type;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration.Metadata
{
	public sealed class AuditMetadataGenerator
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(AuditMetadataGenerator));

		public Cfg.Configuration Cfg { get; private set; }
		public GlobalConfiguration GlobalCfg { get; private set; }
		public AuditEntitiesConfiguration VerEntCfg { get; private set; }
		private readonly XmlElement revisionInfoRelationMapping;

		/*
		 * Generators for different kinds of property values/types.
		 */
		public BasicMetadataGenerator BasicMetadataGenerator { get; private set; }
		private readonly ComponentMetadataGenerator componentMetadataGenerator;
		private readonly IdMetadataGenerator idMetadataGenerator;
		private readonly ToOneRelationMetadataGenerator toOneRelationMetadataGenerator;

		/*
		 * Here information about already generated mappings will be accumulated.
		 */
		public IDictionary<String, EntityConfiguration> EntitiesConfigurations { get; private set; }
		public IDictionary<String, EntityConfiguration> NotAuditedEntitiesConfigurations { get; private set; }

		public AuditEntityNameRegister AuditEntityNameRegister { get; private set; }
		public ClassesAuditingData ClassesAuditingData { get; private set; }

		// Map entity name -> (join descriptor -> element describing the "versioned" join)
		private IDictionary<String, IDictionary<Join, XmlElement>> entitiesJoins;

		public AuditMetadataGenerator(Cfg.Configuration cfg, GlobalConfiguration globalCfg,
									  AuditEntitiesConfiguration verEntCfg,
									  XmlElement revisionInfoRelationMapping,
									  AuditEntityNameRegister auditEntityNameRegister,
									  ClassesAuditingData classesAuditingData)
		{
			Cfg = cfg;
			GlobalCfg = globalCfg;
			VerEntCfg = verEntCfg;
			this.revisionInfoRelationMapping = revisionInfoRelationMapping;

			BasicMetadataGenerator = new BasicMetadataGenerator();
			componentMetadataGenerator = new ComponentMetadataGenerator(this);
			idMetadataGenerator = new IdMetadataGenerator(this);
			toOneRelationMetadataGenerator = new ToOneRelationMetadataGenerator(this);

			AuditEntityNameRegister = auditEntityNameRegister;
			ClassesAuditingData = classesAuditingData;

			EntitiesConfigurations = new Dictionary<String, EntityConfiguration>();
			NotAuditedEntitiesConfigurations = new Dictionary<String, EntityConfiguration>();
			entitiesJoins = new Dictionary<String, IDictionary<Join, XmlElement>>();
		}

		/**
		 * Clones the revision info relation mapping, so that it can be added to other mappings. Also, the name of
		 * the property and the column are set properly.
		 * @return A revision info mapping, which can be added to other mappings (has no parent).
		 */
		private XmlElement CloneAndSetupRevisionInfoRelationMapping(XmlDocument doc)
		{
			var rev_mapping = (XmlElement)doc.ImportNode(revisionInfoRelationMapping, true);
			rev_mapping.SetAttribute("name", VerEntCfg.RevisionFieldName);

			MetadataTools.AddOrModifyColumn(rev_mapping, VerEntCfg.RevisionFieldName);

			return rev_mapping;
		}

		public void AddRevisionInfoRelation(XmlElement any_mapping)
		{
			any_mapping.AppendChild(CloneAndSetupRevisionInfoRelationMapping(any_mapping.OwnerDocument));
		}

		public void AddRevisionType(XmlElement any_mapping)
		{
			var revTypeProperty = MetadataTools.AddProperty(any_mapping, VerEntCfg.RevisionTypePropName,
					VerEntCfg.RevisionTypePropType, true, false);
			revTypeProperty.SetAttribute("type", typeof(RevisionTypeType).AssemblyQualifiedName);
		}

		public void AddValue(XmlElement parent, IValue value, ICompositeMapperBuilder currentMapper, String entityName,
					  EntityXmlMappingData xmlMappingData, PropertyAuditingData propertyAuditingData,
					  bool insertable, bool firstPass)
		{
			var type = value.Type;

			// only first pass
			if (firstPass)
			{
				if (BasicMetadataGenerator.AddBasic(parent, propertyAuditingData, value, currentMapper,
						insertable, false))
				{
					// The property was mapped by the basic generator.
					return;
				}
			}

			if (type is ComponentType)
			{
				// both passes
				componentMetadataGenerator.AddComponent(parent, propertyAuditingData, value, currentMapper,
						entityName, xmlMappingData, firstPass);
			}
			else if (type is ManyToOneType)
			{
				// only second pass
				if (!firstPass)
				{
					toOneRelationMetadataGenerator.AddToOne(parent, propertyAuditingData, value, currentMapper,
							entityName, insertable);
				}
			}
			else if (type is OneToOneType)
			{
				// only second pass
				if (!firstPass)
				{
					toOneRelationMetadataGenerator.AddOneToOneNotOwning(propertyAuditingData, value,
							currentMapper, entityName);
				}
			}
			else if (type is CollectionType)
			{
				// only second pass
				if (!firstPass)
				{
					var collectionMetadataGenerator = new CollectionMetadataGenerator(this,
							(Mapping.Collection)value, currentMapper, entityName, xmlMappingData,
							propertyAuditingData);
					collectionMetadataGenerator.AddCollection();
				}
			}
			else
			{
				if (firstPass)
				{
					// If we got here in the first pass, it means the basic mapper didn't map it, and none of the
					// above branches either.
					ThrowUnsupportedTypeException(type, entityName, propertyAuditingData.Name);
				}
			}
		}

		//@SuppressWarnings({"unchecked"})
		private void AddProperties(XmlElement parent, IEnumerator<Property> properties, ICompositeMapperBuilder currentMapper,
								   ClassAuditingData auditingData, String entityName, EntityXmlMappingData xmlMappingData,
								   bool firstPass)
		{
			while (properties.MoveNext())
			{
				var property = properties.Current;
				var propertyName = property.Name;
				var propertyAuditingData = auditingData.GetPropertyAuditingData(propertyName);
				if (propertyAuditingData != null)
				{
					AddValue(parent, property.Value, currentMapper, entityName, xmlMappingData, propertyAuditingData,
							property.IsInsertable, firstPass);
				}
			}
		}

		private bool CheckPropertiesAudited(IEnumerator<Property> properties, ClassAuditingData auditingData)
		{
			while (properties.MoveNext())
			{
				var property = properties.Current;
				var propertyName = property.Name;
				var propertyAuditingData = auditingData.GetPropertyAuditingData(propertyName);
				if (propertyAuditingData == null)
				{
					return false;
				}
			}

			return true;
		}

		public String GetSchema(string schemaFromAnnotation, Table table)
		{
			// Get the schema from the annotation ...
			var schema = schemaFromAnnotation;
			// ... if empty, try using the default ...
			if (string.IsNullOrEmpty(schema))
			{
				schema = GlobalCfg.DefaultSchemaName;

				// ... if still empty, use the same as the normal table.
				if (string.IsNullOrEmpty(schema))
				{
					schema = table.Schema;
				}
			}
			return schema;
		}

		public string GetCatalog(string catalogFromAnnotation, Table table)
		{
			// Get the catalog from the annotation ...
			var catalog = catalogFromAnnotation;
			// ... if empty, try using the default ...
			if (string.IsNullOrEmpty(catalog))
			{
				catalog = GlobalCfg.DefaultCatalogName;

				// ... if still empty, use the same as the normal table.
				if (string.IsNullOrEmpty(catalog))
				{
					catalog = table.Catalog;
				}
			}
			return catalog;
		}

		private void CreateJoins(PersistentClass pc, XmlElement parent, ClassAuditingData auditingData)
		{
			var JoinElements = new Dictionary<Join, XmlElement>();
			entitiesJoins.Add(pc.EntityName, JoinElements);

			foreach (var join in pc.JoinIterator)
			{
				// Checking if all of the join properties are audited
				if (!CheckPropertiesAudited(join.PropertyIterator.GetEnumerator(), auditingData))
				{
					continue;
				}

				// Determining the table name. If there is no entry in the dictionary, just constructing the table name
				// as if it was an entity (by appending/prepending configured strings).
				var originalTableName = join.Table.Name;
				string auditTableName;
				if (!auditingData.SecondaryTableDictionary.TryGetValue(originalTableName, out auditTableName))
				{
					auditTableName = VerEntCfg.GetAuditEntityName(originalTableName);
				}

				var schema = GetSchema(auditingData.AuditTable.Schema, join.Table);
				var catalog = GetCatalog(auditingData.AuditTable.Catalog, join.Table);

				var joinElement = MetadataTools.CreateJoin(parent, auditTableName, schema, catalog);
				JoinElements.Add(join, joinElement);

				var joinKey = joinElement.OwnerDocument.CreateElement("key");
				joinElement.AppendChild(joinKey);
				MetadataTools.AddColumns(joinKey, join.Key.ColumnIterator.OfType<Column>());
				MetadataTools.AddColumn(joinKey, VerEntCfg.RevisionFieldName, -1, 0, 0, null);
			}
		}

		private void AddJoins(PersistentClass pc, ICompositeMapperBuilder currentMapper, ClassAuditingData auditingData,
							  String entityName, EntityXmlMappingData xmlMappingData, bool firstPass)
		{
			var joins = pc.JoinIterator.GetEnumerator();

			while (joins.MoveNext())
			{
				var join = joins.Current;
				var joinElement = entitiesJoins[entityName][join];

				if (joinElement != null)
				{
					AddProperties(joinElement, join.PropertyIterator.GetEnumerator(), currentMapper, auditingData, entityName,
							xmlMappingData, firstPass);
				}
			}
		}

		private Triple<XmlElement, IExtendedPropertyMapper, String> GenerateMappingData(
				PersistentClass pc, EntityXmlMappingData xmlMappingData, AuditTableData auditTableData,
				IdMappingData idMapper)
		{
			var hasDiscriminator = pc.Discriminator != null;

			var class_mapping = MetadataTools.CreateEntity(xmlMappingData.MainXmlMapping, auditTableData,
					hasDiscriminator ? pc.DiscriminatorValue : null);
			var propertyMapper = new MultiPropertyMapper();

			// Adding the id mapping
			var xmlMp = class_mapping.OwnerDocument.ImportNode(idMapper.XmlMapping,true);
			class_mapping.AppendChild(xmlMp);

			// Checking if there is a discriminator column
			if (hasDiscriminator)
			{
				var discriminator_element = class_mapping.OwnerDocument.CreateElement("discriminator");
				class_mapping.AppendChild(discriminator_element);
				MetadataTools.AddColumns(discriminator_element, pc.Discriminator.ColumnIterator.OfType<Column>());
				discriminator_element.SetAttribute("type", pc.Discriminator.Type.Name);
			}

			// Adding the "revision type" property
			AddRevisionType(class_mapping);

			return Triple<XmlElement, IExtendedPropertyMapper, string>.Make(class_mapping, propertyMapper, null);
		}

		private Triple<XmlElement, IExtendedPropertyMapper, String> GenerateInheritanceMappingData(
				PersistentClass pc, EntityXmlMappingData xmlMappingData, AuditTableData auditTableData,
				String inheritanceMappingType)
		{
			var extendsEntityName = VerEntCfg.GetAuditEntityName(pc.Superclass.EntityName);
			var hasDiscriminator = pc.Discriminator != null;
			var class_mapping = MetadataTools.CreateSubclassEntity(xmlMappingData.MainXmlMapping,
					inheritanceMappingType, auditTableData, extendsEntityName, hasDiscriminator ? pc.DiscriminatorValue : null);

			// The id and revision type is already mapped in the parent

			// Getting the property mapper of the parent - when mapping properties, they need to be included
			var parentEntityName = pc.Superclass.EntityName;

			var parentConfiguration = EntitiesConfigurations[parentEntityName];
			if (parentConfiguration == null)
			{
				throw new MappingException("Entity '" + pc.EntityName + "' is audited, but its superclass: '" +
						parentEntityName + "' is not.");
			}

			var parentPropertyMapper = parentConfiguration.PropertyMapper;
			var propertyMapper = new SubclassPropertyMapper(new MultiPropertyMapper(), parentPropertyMapper);

			return Triple<XmlElement, IExtendedPropertyMapper, String>.Make(class_mapping, propertyMapper, parentEntityName);
		}

		public void GenerateFirstPass(PersistentClass pc, ClassAuditingData auditingData,
									  EntityXmlMappingData xmlMappingData, bool isAudited)
		{
			var schema = GetSchema(auditingData.AuditTable.Schema, pc.Table);
			var catalog = GetCatalog(auditingData.AuditTable.Catalog, pc.Table);

			var entityName = pc.EntityName;
			if (!isAudited)
			{
				var _idMapper = idMetadataGenerator.AddId(pc);

				if (_idMapper == null)
				{
					// Unsupported id mapping, e.g. key-many-to-one. If the entity is used in auditing, an exception
					// will be thrown later on.
					if (log.IsDebugEnabled)
					{
						log.Debug("Unable to create auditing id mapping for entity " + entityName +
							", because of an unsupported Hibernate id mapping (e.g. key-many-to-one).");
					}
					return;
				}

				//ORIG:
				//IExtendedPropertyMapper propertyMapper = null;
				//String parentEntityName = null;
				var _entityCfg = new EntityConfiguration(entityName, _idMapper, null, null);
				NotAuditedEntitiesConfigurations.Add(entityName, _entityCfg);
				return;
			}

			if (log.IsDebugEnabled)
			{
				log.Debug("Generating first-pass auditing mapping for entity " + entityName + ".");
			}

			var auditEntityName = VerEntCfg.GetAuditEntityName(entityName);
			var auditTableName = VerEntCfg.GetAuditTableName(entityName, pc.Table.Name);

			// Registering the audit entity name, now that it is known
			AuditEntityNameRegister.Register(auditEntityName);

			var auditTableData = new AuditTableData(auditEntityName, auditTableName, schema, catalog);

			// Generating a mapping for the id
			var idMapper = idMetadataGenerator.AddId(pc);

			var inheritanceType = InheritanceType.GetForChild(pc);

			// These properties will be read from the mapping data

			Triple<XmlElement, IExtendedPropertyMapper, String> mappingData;

			// Reading the mapping data depending on inheritance type (if any)
			switch (inheritanceType)
			{
				case InheritanceType.Type.NONE:
					mappingData = GenerateMappingData(pc, xmlMappingData, auditTableData, idMapper);
					break;

				case InheritanceType.Type.SINGLE:
					auditTableData = new AuditTableData(auditEntityName, null, schema, catalog);
					mappingData = GenerateInheritanceMappingData(pc, xmlMappingData, auditTableData, "subclass");
					break;

				case InheritanceType.Type.JOINED:
					mappingData = GenerateInheritanceMappingData(pc, xmlMappingData, auditTableData, "joined-subclass");

					// Adding the "key" element with all id columns...
					var keyMapping = mappingData.First.OwnerDocument.CreateElement("key");
					mappingData.First.AppendChild(keyMapping);
					MetadataTools.AddColumns(keyMapping, pc.Table.PrimaryKey.ColumnIterator);

					// ... and the revision number column, read from the revision info relation mapping.
					keyMapping.AppendChild(CloneAndSetupRevisionInfoRelationMapping(keyMapping.OwnerDocument).GetElementsByTagName("column")[0].Clone());
					break;

				case InheritanceType.Type.TABLE_PER_CLASS:
					mappingData = GenerateInheritanceMappingData(pc, xmlMappingData, auditTableData, "union-subclass");

					break;

				default:
					throw new AssertionFailure("Envers.NET: AuditMetadataGenerator.GenerateFirstPass: Impossible enum value.");
			}

			var class_mapping = mappingData.First;
			var propertyMapper = mappingData.Second;
			var parentEntityName = mappingData.Third;

			xmlMappingData.ClassMapping = class_mapping;

			// Mapping unjoined properties
			AddProperties(class_mapping, pc.UnjoinedPropertyIterator.GetEnumerator(), propertyMapper,
					auditingData, pc.EntityName, xmlMappingData,
					true);

			// Creating and mapping joins (first pass)
			CreateJoins(pc, class_mapping, auditingData);
			AddJoins(pc, propertyMapper, auditingData, pc.EntityName, xmlMappingData, true);

			// Storing the generated configuration
			var entityCfg = new EntityConfiguration(auditEntityName, idMapper,
					propertyMapper, parentEntityName);
			EntitiesConfigurations.Add(pc.EntityName, entityCfg);
		}

		public void GenerateSecondPass(PersistentClass pc, ClassAuditingData auditingData,
									   EntityXmlMappingData xmlMappingData) {
			var entityName = pc.EntityName;
			if (log.IsDebugEnabled)
			{
				log.Debug("Generating second-pass auditing mapping for entity " + entityName + ".");
			}

			var propertyMapper = EntitiesConfigurations[entityName].PropertyMapper;

			// Mapping unjoined properties
			var parent = xmlMappingData.ClassMapping;

			AddProperties(parent, pc.UnjoinedPropertyIterator.GetEnumerator(),
					propertyMapper, auditingData, entityName, xmlMappingData, false);

			// Mapping joins (second pass)
			AddJoins(pc, propertyMapper, auditingData, entityName, xmlMappingData, false);
		}

		// Getters for generators and configuration

		public void ThrowUnsupportedTypeException(IType type, String entityName, String propertyName)
		{
			var message = "Type not supported for auditing: " + type.Name +
					", on entity " + entityName + ", property '" + propertyName + "'.";

			throw new MappingException(message);
		}

		/**
		 * Reads the id mapping data of a referenced entity.
		 * @param entityName Name of the entity which is the source of the relation.
		 * @param referencedEntityName Name of the entity which is the target of the relation.
		 * @param propertyAuditingData Auditing data of the property that is the source of the relation.
		 * @param allowNotAuditedTarget Are not-audited target entities allowed.
		 * @throws MappingException If a relation from an audited to a non-audited entity is detected, which is not
		 * mapped using {@link RelationTargetAuditMode#NOT_AUDITED}.
		 * @return The id mapping data of the related entity. 
		 */
		public IdMappingData GetReferencedIdMappingData(String entityName, String referencedEntityName,
												PropertyAuditingData propertyAuditingData,
												bool allowNotAuditedTarget)
		{
			EntityConfiguration configuration;
			if (EntitiesConfigurations.Keys.Contains(referencedEntityName))
				configuration = EntitiesConfigurations[referencedEntityName];
			else
			{
				var relationTargetAuditMode = propertyAuditingData.getRelationTargetAuditMode();

				if (!NotAuditedEntitiesConfigurations.Keys.Contains(referencedEntityName) ||
					!allowNotAuditedTarget || !RelationTargetAuditMode.NOT_AUDITED.Equals(relationTargetAuditMode))
				{
					throw new MappingException("An audited relation from " + entityName + "."
							+ propertyAuditingData.Name + " to a not audited entity " + referencedEntityName + "!"
							+ (allowNotAuditedTarget ?
								" Such mapping is possible, but has to be explicitly defined using [Audited(TargetAuditMode = RelationTargetAuditMode.NOT_AUDITED)]." :
								string.Empty));
				}
				configuration = NotAuditedEntitiesConfigurations[referencedEntityName];
			}
			return configuration.IdMappingData;
		}
	}
}


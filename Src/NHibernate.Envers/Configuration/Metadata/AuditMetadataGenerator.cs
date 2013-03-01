using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Exceptions;
using NHibernate.Mapping;
using NHibernate.Type;
using System;

namespace NHibernate.Envers.Configuration.Metadata
{
	public sealed class AuditMetadataGenerator
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(AuditMetadataGenerator));

		public Cfg.Configuration Cfg { get; private set; }
		public GlobalConfiguration GlobalCfg { get; private set; }
		public AuditEntitiesConfiguration VerEntCfg { get; private set; }
		private readonly XElement revisionInfoRelationMapping;
		private readonly IMetaDataStore _metaDataStore;

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
		public IDictionary<string, EntityConfiguration> EntitiesConfigurations { get; private set; }
		public IDictionary<string, EntityConfiguration> NotAuditedEntitiesConfigurations { get; private set; }
		public AuditEntityNameRegister AuditEntityNameRegister { get; private set; }

		// Map entity name -> (join descriptor -> element describing the "versioned" join)
		private readonly IDictionary<string, IDictionary<Join, XElement>> entitiesJoins;

		public AuditMetadataGenerator(IMetaDataStore metaDataStore, 
										Cfg.Configuration cfg,
										GlobalConfiguration globalCfg,
										AuditEntitiesConfiguration verEntCfg,
										XElement revisionInfoRelationMapping,
										AuditEntityNameRegister auditEntityNameRegister)
		{
			Cfg = cfg;
			GlobalCfg = globalCfg;
			VerEntCfg = verEntCfg;
			_metaDataStore = metaDataStore;
			this.revisionInfoRelationMapping = revisionInfoRelationMapping;
			BasicMetadataGenerator = new BasicMetadataGenerator();
			componentMetadataGenerator = new ComponentMetadataGenerator(this);
			idMetadataGenerator = new IdMetadataGenerator(this);
			toOneRelationMetadataGenerator = new ToOneRelationMetadataGenerator(this);
			AuditEntityNameRegister = auditEntityNameRegister;
			EntitiesConfigurations = new Dictionary<string, EntityConfiguration>();
			NotAuditedEntitiesConfigurations = new Dictionary<string, EntityConfiguration>();
			entitiesJoins = new Dictionary<string, IDictionary<Join, XElement>>();
		}


		/// <summary>
		///  Clones the revision info relation mapping, so that it can be added to other mappings. Also, the name of
		///  the property and the column are set properly.
		/// </summary>
		/// <returns>A revision info mapping, which can be added to other mappings (has no parent).</returns>
		private XElement cloneAndSetupRevisionInfoRelationMapping()
		{
			var revMapping = new XElement(revisionInfoRelationMapping);
			revMapping.Add(new XAttribute("name", VerEntCfg.RevisionFieldName));

			MetadataTools.AddOrModifyColumn(revMapping, VerEntCfg.RevisionFieldName);

			return revMapping;
		}

		public void AddRevisionInfoRelation(XElement anyMapping)
		{
			anyMapping.Add(cloneAndSetupRevisionInfoRelationMapping());
		}

		public void AddRevisionType(XElement anyMapping, XElement anyMappingEnd)
		{
			var partOfId = anyMapping != anyMappingEnd;
			var revTypeProperty = MetadataTools.AddProperty(anyMapping, VerEntCfg.RevisionTypePropName,
					typeof(RevisionTypeType).AssemblyQualifiedName, true, partOfId);
			if (!partOfId)
			{
				revTypeProperty.Add(new XAttribute("not-null", "true"));
			}
			GlobalCfg.AuditStrategy.AddExtraRevisionMapping(anyMappingEnd, revisionInfoRelationMapping);
		}

		private void addValueInFirstPass(XElement parent, IValue value, ICompositeMapperBuilder currentMapper, string entityName,
					  EntityXmlMappingData xmlMappingData, PropertyAuditingData propertyAuditingData,
					  bool insertable, bool processModifiedFlag)
		{
			var type = value.Type;

			if (BasicMetadataGenerator.AddBasic(parent, propertyAuditingData, value, currentMapper, insertable, false))
			{
				// The property was mapped by the basic generator.
			}
			else if (type is ComponentType)
			{
				componentMetadataGenerator.AddComponent(parent, propertyAuditingData, value, currentMapper, entityName,
																	 xmlMappingData, true, insertable);
			}
			else
			{
				if (!processedInSecondPass(type))
				{
					// If we got here in the first pass, it means the basic mapper didn't map it, and none of the
					// above branches either.
					ThrowUnsupportedTypeException(type, entityName, propertyAuditingData.Name);
				}
				return;
			}
			addModifiedFlagIfNeeded(parent, propertyAuditingData, processModifiedFlag);
		}

		private static bool processedInSecondPass(IType type)
		{
			return type is ComponentType ||
					 type is ManyToOneType ||
					 type is OneToOneType ||
					 type is CollectionType;
		}

		private void addValueInSecondPass(XElement parent, IValue value, ICompositeMapperBuilder currentMapper, string entityName,
																									  EntityXmlMappingData xmlMappingData, PropertyAuditingData propertyAuditingData,
																									  bool insertable, bool processModifiedFlag)
		{
			var type = value.Type;

			if (type is ComponentType)
			{
				componentMetadataGenerator.AddComponent(parent, propertyAuditingData, value, currentMapper,
						entityName, xmlMappingData, false, insertable);
				return;// mod flag field has been already generated in first pass
			}
			else if (type is ManyToOneType)
			{
				toOneRelationMetadataGenerator.AddToOne(parent, propertyAuditingData, value, currentMapper, entityName, insertable);
			}
			else if (type is OneToOneType)
			{
				var oneToOne = (OneToOne)value;
				if (oneToOne.ReferencedPropertyName != null)
				{
					toOneRelationMetadataGenerator.AddOneToOneNotOwning(propertyAuditingData, value, currentMapper, entityName);	
				}
				else
				{
					// @OneToOne relation marked with @PrimaryKeyJoinColumn
					toOneRelationMetadataGenerator.AddOneToOnePrimaryKeyJoinColumn(propertyAuditingData, value,
																		currentMapper, entityName, insertable);
				}
			}
			else if (type is CollectionType)
			{
				
				var collectionMetadataGenerator = new CollectionMetadataGenerator(_metaDataStore, this, (Mapping.Collection) value, currentMapper, entityName,
																										xmlMappingData, propertyAuditingData);
				collectionMetadataGenerator.AddCollection();
			}
			else
			{
				return;
			}
			addModifiedFlagIfNeeded(parent, propertyAuditingData, processModifiedFlag);
		}

		private void addModifiedFlagIfNeeded(XElement parent, PropertyAuditingData propertyAuditingData, bool processModifiedFlag)
		{
			if (processModifiedFlag && propertyAuditingData.UsingModifiedFlag)
			{
				MetadataTools.AddModifiedFlagProperty(parent, propertyAuditingData.Name, GlobalCfg.ModifiedFlagSuffix);
			}
		}

		public void AddValue(XElement parent, IValue value, ICompositeMapperBuilder currentMapper, string entityName,
												 EntityXmlMappingData xmlMappingData, PropertyAuditingData propertyAuditingData,
												 bool insertable, bool firstPass, bool processModifiedFlag)
		{
			if (firstPass)
			{
				addValueInFirstPass(parent, value, currentMapper, entityName, xmlMappingData, propertyAuditingData, insertable, processModifiedFlag);
			}
			else
			{
				addValueInSecondPass(parent, value, currentMapper, entityName, xmlMappingData, propertyAuditingData, insertable, processModifiedFlag);
			}
		}

		private void addProperties(XElement parent, IEnumerable<Property> properties, ICompositeMapperBuilder currentMapper,
									ClassAuditingData auditingData, string entityName, EntityXmlMappingData xmlMappingData,
									bool firstPass)
		{
			foreach (var property in properties)
			{
				var propertyName = property.Name;
				var propertyAuditingData = auditingData.GetPropertyAuditingData(propertyName);
				if (propertyAuditingData != null)
				{
					AddValue(parent, property.Value, currentMapper, entityName, xmlMappingData, propertyAuditingData,
							property.IsInsertable, firstPass, true);
				}
			}
		}

		private static bool checkAnyPropertyAudited(IEnumerable<Property> properties, ClassAuditingData auditingData)
		{
			foreach (var property in properties)
			{
				var propertyName = property.Name;
				var propertyAuditingData = auditingData.GetPropertyAuditingData(propertyName);
				if (propertyAuditingData != null)
				{
					return true;
				}
			}

			return false;
		}

		public string GetSchema(string schemaFromAnnotation, Table table)
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

		private void createJoins(PersistentClass pc, XElement parent, ClassAuditingData auditingData)
		{
			var joinElements = new Dictionary<Join, XElement>();
			entitiesJoins.Add(pc.EntityName, joinElements);

			foreach (var join in pc.JoinIterator)
			{
				// Checking if any of the join properties are audited
				if (!checkAnyPropertyAudited(join.PropertyIterator, auditingData))
				{
					continue;
				}

				// Determining the table name. If there is no entry in the dictionary, just constructing the table name
				// as if it was an entity (by appending/prepending configured strings).
				string auditTableName;
				if (!auditingData.JoinTableDictionary.TryGetValue(join.Table.Name, out auditTableName))
				{
					auditTableName = VerEntCfg.JoinTableName(join);
				}

				var schema = GetSchema(auditingData.AuditTable.Schema, join.Table);
				var catalog = GetCatalog(auditingData.AuditTable.Catalog, join.Table);				

				var joinElement = MetadataTools.CreateJoin(parent, auditTableName, schema, catalog);
				joinElements.Add(join, joinElement);

				var joinKey = new XElement(MetadataTools.CreateElementName("key"));
				joinElement.Add(joinKey);
				MetadataTools.AddColumns(joinKey, join.Key.ColumnIterator.OfType<Column>());
				MetadataTools.AddColumn(joinKey, VerEntCfg.RevisionFieldName, -1, -1, -1, null, false);
			}
		}

		private void addJoins(PersistentClass pc, ICompositeMapperBuilder currentMapper, ClassAuditingData auditingData,
							  string entityName, EntityXmlMappingData xmlMappingData, bool firstPass)
		{
			var entityJoin = entitiesJoins[entityName];
			foreach (var join in pc.JoinIterator)
			{
				XElement joinElement;
				if (entityJoin.TryGetValue(join, out joinElement))
				{
					addProperties(joinElement, join.PropertyIterator, currentMapper, auditingData, entityName, xmlMappingData, firstPass);
				}
			}
		}

		private Tuple<XElement, IExtendedPropertyMapper, string> generateMappingData(
				PersistentClass pc, EntityXmlMappingData xmlMappingData, AuditTableData auditTableData,
				IdMappingData idMapper)
		{
			var hasDiscriminator = pc.Discriminator != null;

			var classMapping = MetadataTools.CreateEntity(xmlMappingData.MainXmlMapping, auditTableData,
					hasDiscriminator ? pc.DiscriminatorValue : null, pc.IsAbstract.HasValue && pc.IsAbstract.Value);
			var propertyMapper = new MultiPropertyMapper();

			// Adding the id mapping
			classMapping.Add(idMapper.XmlMapping);

			// Checking if there is a discriminator column
			if (hasDiscriminator)
			{
				var discriminatorElement = new XElement(MetadataTools.CreateElementName("discriminator"));
				classMapping.Add(discriminatorElement);
				// Database column or SQL formula allowed to distinguish entity types
				MetadataTools.AddColumnsOrFormulas(discriminatorElement, pc.Discriminator.ColumnIterator);
				discriminatorElement.Add(new XAttribute("type", pc.Discriminator.Type.Name));
			}

			// Adding the "revision type" property
			AddRevisionType(classMapping, classMapping);

			return new Tuple<XElement, IExtendedPropertyMapper, string>(classMapping, propertyMapper, null);
		}

		private Tuple<XElement, IExtendedPropertyMapper, string> generateInheritanceMappingData(
				PersistentClass pc, EntityXmlMappingData xmlMappingData, AuditTableData auditTableData,
				string inheritanceMappingType)
		{
			var extendsEntityName = VerEntCfg.GetAuditEntityName(pc.Superclass.EntityName);
			var hasDiscriminator = pc.Discriminator != null;
			var classMapping = MetadataTools.CreateSubclassEntity(xmlMappingData.MainXmlMapping,
					inheritanceMappingType, auditTableData, extendsEntityName, hasDiscriminator ? pc.DiscriminatorValue : null,
					pc.IsAbstract.HasValue && pc.IsAbstract.Value);

			// The id and revision type is already mapped in the parent

			// Getting the property mapper of the parent - when mapping properties, they need to be included
			var parentEntityName = pc.Superclass.EntityName;

			EntityConfiguration parentConfiguration;
			if (!EntitiesConfigurations.TryGetValue(parentEntityName, out parentConfiguration))
			{
				throw new MappingException("Entity '" + pc.EntityName + "' is audited, but its superclass: '" + parentEntityName + "' is not.");
			}

			var parentPropertyMapper = parentConfiguration.PropertyMapper;
			var propertyMapper = new SubclassPropertyMapper(new MultiPropertyMapper(), parentPropertyMapper);

			return new Tuple<XElement, IExtendedPropertyMapper, string>(classMapping, propertyMapper, parentEntityName);
		}

		public void GenerateFirstPass(PersistentClass pc, ClassAuditingData auditingData,
									  EntityXmlMappingData xmlMappingData, bool isAudited)
		{
			var schema = GetSchema(auditingData.AuditTable.Schema, pc.Table);
			var catalog = GetCatalog(auditingData.AuditTable.Catalog, pc.Table);
			Func<System.Type, object> factory = auditingData.Factory.Factory.Instantiate;

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
						log.DebugFormat("Unable to create auditing id mapping for entity {0}" +
							", because of an unsupported Hibernate id mapping (e.g. key-many-to-one).", entityName);
					}
					return;
				}

				//ORIG:
				//IExtendedPropertyMapper propertyMapper = null;
				//String parentEntityName = null;
				var _entityCfg = new EntityConfiguration(entityName, pc.ClassName, _idMapper, null, null, factory);
				NotAuditedEntitiesConfigurations.Add(entityName, _entityCfg);
				return;
			}

			if (log.IsDebugEnabled)
			{
				log.DebugFormat("Generating first-pass auditing mapping for entity {0}.", entityName);
			}

			var auditEntityName = VerEntCfg.GetAuditEntityName(entityName);
			var auditTableName = VerEntCfg.AuditTableName(entityName, pc);

			// Registering the audit entity name, now that it is known
			AuditEntityNameRegister.Register(auditEntityName);

			var auditTableData = new AuditTableData(auditEntityName, auditTableName, schema, catalog);

			// Generating a mapping for the id
			var idMapper = idMetadataGenerator.AddId(pc);
			if (idMapper == null)
			{
				throw new AuditException("Id mapping for type " + pc.ClassName +
				                         " is currently not supported in Envers. If you need composite-id, use 'Components as composite identifiers'.");
			}

			var inheritanceType = pc.GetInheritanceType();

			// These properties will be read from the mapping data

			Tuple<XElement, IExtendedPropertyMapper, string> mappingData;

			// Reading the mapping data depending on inheritance type (if any)
			switch (inheritanceType)
			{
				case InheritanceType.None:
					mappingData = generateMappingData(pc, xmlMappingData, auditTableData, idMapper);
					break;

				case InheritanceType.Single:
					auditTableData = new AuditTableData(auditEntityName, null, schema, catalog);
					mappingData = generateInheritanceMappingData(pc, xmlMappingData, auditTableData, "subclass");
					break;

				case InheritanceType.Joined:
					mappingData = generateInheritanceMappingData(pc, xmlMappingData, auditTableData, "joined-subclass");

					// Adding the "key" element with all id columns...
					var keyMapping = new XElement(MetadataTools.CreateElementName("key"));
					mappingData.Item1.Add(keyMapping);
					MetadataTools.AddColumns(keyMapping, pc.Table.PrimaryKey.ColumnIterator);

					// ... and the revision number column, read from the revision info relation mapping.
					keyMapping.Add(cloneAndSetupRevisionInfoRelationMapping().Element(MetadataTools.CreateElementName("column")));
					break;

				case InheritanceType.TablePerClass:
					mappingData = generateInheritanceMappingData(pc, xmlMappingData, auditTableData, "union-subclass");

					break;

				default:
					throw new AssertionFailure("AuditMetadataGenerator.GenerateFirstPass: Impossible enum value.");
			}

			var classMapping = mappingData.Item1;
			var propertyMapper = mappingData.Item2;
			var parentEntityName = mappingData.Item3;

			xmlMappingData.ClassMapping = classMapping;

			// Mapping unjoined properties
			addProperties(classMapping, pc.UnjoinedPropertyIterator, propertyMapper,
					auditingData, pc.EntityName, xmlMappingData,
					true);

			// Creating and mapping joins (first pass)
			createJoins(pc, classMapping, auditingData);
			addJoins(pc, propertyMapper, auditingData, pc.EntityName, xmlMappingData, true);

			// Storing the generated configuration
			var entityCfg = new EntityConfiguration(auditEntityName, pc.ClassName, idMapper,
					propertyMapper, parentEntityName, factory);
			EntitiesConfigurations.Add(pc.EntityName, entityCfg);
		}

		public void GenerateSecondPass(PersistentClass pc, ClassAuditingData auditingData,
										EntityXmlMappingData xmlMappingData)
		{
			var entityName = pc.EntityName;
			if (log.IsDebugEnabled)
			{
				log.DebugFormat("Generating second-pass auditing mapping for entity {0}.", entityName);
			}

			var propertyMapper = EntitiesConfigurations[entityName].PropertyMapper;

			// Mapping unjoined properties
			var parent = xmlMappingData.ClassMapping;

			addProperties(parent, pc.UnjoinedPropertyIterator,
					propertyMapper, auditingData, entityName, xmlMappingData, false);

			// Mapping joins (second pass)
			addJoins(pc, propertyMapper, auditingData, entityName, xmlMappingData, false);
		}

		// Getters for generators and configuration

		public void ThrowUnsupportedTypeException(IType type, string entityName, string propertyName)
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
		 * mapped using {@link RelationTargetAuditMode#NotAudited}.
		 * @return The id mapping data of the related entity. 
		 */
		public IdMappingData GetReferencedIdMappingData(string entityName, string referencedEntityName,
												PropertyAuditingData propertyAuditingData,
												bool allowNotAuditedTarget)
		{
			EntityConfiguration configuration;
			if (EntitiesConfigurations.Keys.Contains(referencedEntityName))
				configuration = EntitiesConfigurations[referencedEntityName];
			else
			{
				var relationTargetAuditMode = propertyAuditingData.RelationTargetAuditMode;

				if (!NotAuditedEntitiesConfigurations.Keys.Contains(referencedEntityName) ||
					!allowNotAuditedTarget || !RelationTargetAuditMode.NotAudited.Equals(relationTargetAuditMode))
				{
					throw new MappingException("An audited relation from " + entityName + "."
							+ propertyAuditingData.Name + " to a not audited entity " + referencedEntityName + "!"
							+ (allowNotAuditedTarget ?
								" Such mapping is possible, but has to be explicitly defined using [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]." :
								string.Empty));
				}
				configuration = NotAuditedEntitiesConfigurations[referencedEntityName];
			}
			return configuration.IdMappingData;
		}
	}
}


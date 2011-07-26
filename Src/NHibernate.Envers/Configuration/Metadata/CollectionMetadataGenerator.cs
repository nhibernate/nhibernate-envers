using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Component;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Tools;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Envers.Configuration.Metadata
{
	public sealed class CollectionMetadataGenerator 
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(CollectionMetadataGenerator));

		private readonly AuditMetadataGenerator mainGenerator;
		private readonly string propertyName;
		private readonly Mapping.Collection propertyValue;
		private readonly ICompositeMapperBuilder currentMapper;
		private readonly string referencingEntityName;
		private readonly EntityXmlMappingData xmlMappingData;
		private readonly PropertyAuditingData propertyAuditingData;
		private readonly EntityConfiguration referencingEntityConfiguration;

		/// <summary>
		/// Null if this collection isn't a relation to another entity.
		/// </summary>
		private readonly string referencedEntityName;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="mainGenerator">Main generator, giving access to configuration and the basic mapper.</param>
		/// <param name="propertyValue">Value of the collection, as mapped by Hibernate.</param>
		/// <param name="currentMapper">Mapper, to which the appropriate {@link org.hibernate.envers.entities.mapper.PropertyMapper} will be added.</param>
		/// <param name="referencingEntityName">Name of the entity that owns this collection.</param>
		/// <param name="xmlMappingData">In case this collection requires a middle table, additional mapping documents will be created using this object.</param>
		/// <param name="propertyAuditingData">
		/// Property auditing (meta-)data. Among other things, holds the name of the
		/// property that references the collection in the referencing entity, the user data for middle (join)
		/// table and the value of the <code>@MapKey</code> annotation, if there was one.
		/// </param>
		public CollectionMetadataGenerator(AuditMetadataGenerator mainGenerator,
											Mapping.Collection propertyValue, 
											ICompositeMapperBuilder currentMapper,
											string referencingEntityName, 
											EntityXmlMappingData xmlMappingData,
											PropertyAuditingData propertyAuditingData) 
		{
			this.mainGenerator = mainGenerator;
			this.propertyValue = propertyValue;
			this.currentMapper = currentMapper;
			this.referencingEntityName = referencingEntityName;
			this.xmlMappingData = xmlMappingData;
			this.propertyAuditingData = propertyAuditingData;

			propertyName = propertyAuditingData.Name;

			referencingEntityConfiguration = mainGenerator.EntitiesConfigurations[referencingEntityName];
			if (referencingEntityConfiguration == null) 
			{
				throw new MappingException("Unable to read auditing configuration for " + referencingEntityName + "!");
			}

			referencedEntityName = MappingTools.ReferencedEntityName(propertyValue.Element);
		}

		public void AddCollection() 
		{
			var type = propertyValue.Type;

			var oneToManyAttachedType = type is BagType || type is SetType || type is MapType || type is ListType;
			var inverseOneToMany = (propertyValue.Element is OneToMany) && (propertyValue.IsInverse);
			var fakeOneToManyBidirectional = (propertyValue.Element is OneToMany) && (propertyAuditingData.AuditMappedBy != null);

			if (oneToManyAttachedType && (inverseOneToMany || fakeOneToManyBidirectional)) 
			{
				// A one-to-many relation mapped using @ManyToOne and @OneToMany(mappedBy="...")
				AddOneToManyAttached(fakeOneToManyBidirectional);
			} 
			else 
			{
				// All other kinds of relations require a middle (join) table.
				AddWithMiddleTable();
			}
		}

		private MiddleIdData CreateMiddleIdData(IdMappingData idMappingData, string prefix, string entityName) 
		{
			return new MiddleIdData(mainGenerator.VerEntCfg, idMappingData, prefix, entityName, 
				mainGenerator.EntitiesConfigurations.ContainsKey(entityName));
		}

		private void AddOneToManyAttached(bool fakeOneToManyBidirectional) 
		{
			log.Debug("Adding audit mapping for property " + referencingEntityName + "." + propertyName +
					": one-to-many collection, using a join column on the referenced entity.");

			var mappedBy = GetMappedBy(propertyValue);

			var referencedIdMapping = mainGenerator.GetReferencedIdMappingData(referencingEntityName,
						referencedEntityName, propertyAuditingData, false);
			var referencingIdMapping = referencingEntityConfiguration.IdMappingData;

			// Generating the id mappers data for the referencing side of the relation.
			var referencingIdData = CreateMiddleIdData(referencingIdMapping,
					mappedBy + "_", referencingEntityName);

			// And for the referenced side. The prefixed mapper won't be used (as this collection isn't persisted
			// in a join table, so the prefix value is arbitrary).
			var referencedIdData = CreateMiddleIdData(referencedIdMapping,
					null, referencedEntityName);

			// Generating the element mapping.
			var elementComponentData = new MiddleComponentData(
					new MiddleRelatedComponentMapper(referencedIdData), 0);

			// Generating the index mapping, if an index exists. It can only exists in case a javax.persistence.MapKey
			// annotation is present on the entity. So the middleEntityXml will be not be used. The queryGeneratorBuilder
			// will only be checked for nullnes.
			var indexComponentData = AddIndex(null, null);

			// Generating the query generator - it should read directly from the related entity.
			var queryGenerator = new OneAuditEntityQueryGenerator(mainGenerator.GlobalCfg, mainGenerator.VerEntCfg,
					mainGenerator.AuditStrategy, referencingIdData, referencedEntityName, referencedIdData);

			// Creating common mapper data.
			var commonCollectionMapperData = new CommonCollectionMapperData(
					mainGenerator.VerEntCfg, referencedEntityName,
					propertyAuditingData.GetPropertyData(), 
					referencingIdData, queryGenerator);

			IPropertyMapper fakeBidirectionalRelationMapper;
			IPropertyMapper fakeBidirectionalRelationIndexMapper;
			if (fakeOneToManyBidirectional)
			{
				// In case of a fake many-to-one bidirectional relation, we have to generate a mapper which maps
				// the mapped-by property name to the id of the related entity (which is the owner of the collection).
				var auditMappedBy = propertyAuditingData.AuditMappedBy;

				// Creating a prefixed relation mapper.
				var relMapper = referencingIdMapping.IdMapper.PrefixMappedProperties(
						MappingTools.CreateToOneRelationPrefix(auditMappedBy));

				fakeBidirectionalRelationMapper = new ToOneIdMapper(
						relMapper,
					// The mapper will only be used to map from entity to map, so no need to provide other details
					// when constructing the PropertyData.
						new PropertyData(auditMappedBy, null, null, ModificationStore.None),
						referencedEntityName, false);

				// Checking if there's an index defined. If so, adding a mapper for it.
				if (propertyAuditingData.PositionMappedBy != null)
				{
					var positionMappedBy = propertyAuditingData.PositionMappedBy;
					fakeBidirectionalRelationIndexMapper = new SinglePropertyMapper(new PropertyData(positionMappedBy, null, null, ModificationStore.None));

					// Also, overwriting the index component data to properly read the index.
					indexComponentData = new MiddleComponentData(new MiddleStraightComponentMapper(positionMappedBy), 0);
				}
				else
				{
					fakeBidirectionalRelationIndexMapper = null;
				}
			}
			else
			{
				fakeBidirectionalRelationMapper = null;
				fakeBidirectionalRelationIndexMapper = null;
			}

			// Checking the type of the collection and adding an appropriate mapper.
			AddMapper(commonCollectionMapperData, elementComponentData, indexComponentData);

			// Storing information about this relation.
			referencingEntityConfiguration.AddToManyNotOwningRelation(propertyName, mappedBy,
					referencedEntityName, referencingIdData.PrefixedMapper, fakeBidirectionalRelationMapper,
					fakeBidirectionalRelationIndexMapper);
		}

		/// <summary>
		/// Adds mapping of the id of a related entity to the given xml mapping, prefixing the id with the given prefix.
		/// </summary>
		/// <param name="xmlMapping">Mapping, to which to add the xml.</param>
		/// <param name="prefix">Prefix for the names of properties which will be prepended to properties that form the id.</param>
		/// <param name="columnNames">Column names that will be used for properties that form the id.</param>
		/// <param name="relatedIdMapping">Id mapping data of the related entity.</param>
		private static void addRelatedToXmlMapping(XmlElement xmlMapping, string prefix,
											IEnumerator<string> columnNames,
											IdMappingData relatedIdMapping) 
		{
			var properties = (XmlElement) relatedIdMapping.XmlRelationMapping.Clone();
			MetadataTools.PrefixNamesInPropertyElement(properties, prefix, columnNames, true, true);
			foreach (XmlNode idProperty in properties.ChildNodes)
			{
				var tempNode = xmlMapping.OwnerDocument.ImportNode(idProperty, true);
				xmlMapping.AppendChild(tempNode);
			}
		}

		private static string middleTableName(Mapping.Collection value, string entityName) 
		{
			// We check how Hibernate maps the collection.
			if (value.Element is OneToMany && !value.IsInverse) 
			{
				// This must be a @JoinColumn+@OneToMany mapping. Generating the table name, as Hibernate doesn't use a
				// middle table for mapping this relation.
				var refEntName = MappingTools.ReferencedEntityName(value.Element);
				return entityName.Substring(entityName.LastIndexOf(".") + 1) + "_" + 
					refEntName.Substring(refEntName.LastIndexOf(".") + 1);
			}
			// Hibernate uses a middle table for mapping this relation, so we get it's name directly.
			return value.CollectionTable.Name;
		}

		private void AddWithMiddleTable() 
		{
			log.Debug("Adding audit mapping for property " + referencingEntityName + "." + propertyName +
					": collection with a join table.");

			// Generating the name of the middle table
			string auditMiddleTableName;
			string auditMiddleEntityName;
			if (!string.IsNullOrEmpty(propertyAuditingData.JoinTable.TableName))
			{
				auditMiddleTableName = propertyAuditingData.JoinTable.TableName;
				auditMiddleEntityName = propertyAuditingData.JoinTable.TableName;
			}
			else
			{
				var middleTableName = CollectionMetadataGenerator.middleTableName(propertyValue, referencingEntityName);
				auditMiddleTableName = mainGenerator.VerEntCfg.GetAuditTableName(null, middleTableName);
				auditMiddleEntityName = mainGenerator.VerEntCfg.GetAuditEntityName(middleTableName);
			}

			log.Debug("Using join table name: " + auditMiddleTableName);

			// Generating the XML mapping for the middle entity, only if the relation isn't inverse.
			// If the relation is inverse, will be later checked by comparing middleEntityXml with null.
			XmlElement middleEntityXml;
			if (!propertyValue.IsInverse)
			{
				// Generating a unique middle entity name
				auditMiddleEntityName = mainGenerator.AuditEntityNameRegister.CreateUnique(auditMiddleEntityName);

				// Registering the generated name
				mainGenerator.AuditEntityNameRegister.Register(auditMiddleEntityName);

				middleEntityXml = CreateMiddleEntityXml(auditMiddleTableName, auditMiddleEntityName, propertyValue.Where);
			}
			else
			{
				middleEntityXml = null;
			}

			// ******
			// Generating the mapping for the referencing entity (it must be an entity).
			// ******
			// Getting the id-mapping data of the referencing entity (the entity that "owns" this collection).
			var referencingIdMapping = referencingEntityConfiguration.IdMappingData;

			// Only valid for an inverse relation; null otherwise.
			string mappedBy;

			// The referencing prefix is always for a related entity. So it has always the "_" at the end added.
			string referencingPrefixRelated;
			string referencedPrefix;

			if (propertyValue.IsInverse)
			{
				// If the relation is inverse, then referencedEntityName is not null.
				mappedBy = GetMappedBy(propertyValue.CollectionTable, mainGenerator.Cfg.GetClassMapping(referencedEntityName));

				referencingPrefixRelated = mappedBy + "_";
				referencedPrefix = StringTools.GetLastComponent(referencedEntityName);
			}
			else
			{
				mappedBy = null;

				referencingPrefixRelated = StringTools.GetLastComponent(referencingEntityName) + "_";
				referencedPrefix = referencedEntityName == null ? "element" : propertyName;
			}

			// Storing the id data of the referencing entity: original mapper, prefixed mapper and entity name.
			var referencingIdData = CreateMiddleIdData(referencingIdMapping, referencingPrefixRelated, referencingEntityName);

			// Creating a query generator builder, to which additional id data will be added, in case this collection
			// references some entities (either from the element or index). At the end, this will be used to build
			// a query generator to read the raw data collection from the middle table.
			var queryGeneratorBuilder = new QueryGeneratorBuilder(mainGenerator.GlobalCfg,
					mainGenerator.VerEntCfg, mainGenerator.AuditStrategy, referencingIdData, auditMiddleEntityName);

			// Adding the XML mapping for the referencing entity, if the relation isn't inverse.
			if (middleEntityXml != null)
			{
				// Adding related-entity (in this case: the referencing's entity id) id mapping to the xml.
				addRelatedToXmlMapping(middleEntityXml, referencingPrefixRelated,
						MetadataTools.GetColumnNameEnumerator(propertyValue.Key.ColumnIterator),
						referencingIdMapping);
			}

			// ******
			// Generating the element mapping.
			// ******
			var elementComponentData = addValueToMiddleTable(propertyValue.Element, middleEntityXml,
					queryGeneratorBuilder, referencedPrefix, propertyAuditingData.JoinTable.InverseJoinColumns);

			// ******
			// Generating the index mapping, if an index exists.
			// ******
			var indexComponentData = AddIndex(middleEntityXml, queryGeneratorBuilder);

			// ******
			// Generating the property mapper.
			// ******
			// Building the query generator.
			var queryGenerator = queryGeneratorBuilder.Build(new Collection<MiddleComponentData>{elementComponentData, indexComponentData});

			// Creating common data
			var commonCollectionMapperData = new CommonCollectionMapperData(
					mainGenerator.VerEntCfg, auditMiddleEntityName,
					propertyAuditingData.GetPropertyData(),
					referencingIdData, queryGenerator);

			// Checking the type of the collection and adding an appropriate mapper.
			AddMapper(commonCollectionMapperData, elementComponentData, indexComponentData);

			// ******
			// Storing information about this relation.
			// ******
			StoreMiddleEntityRelationInformation(mappedBy);
		}

		private MiddleComponentData AddIndex(XmlElement middleEntityXml, QueryGeneratorBuilder queryGeneratorBuilder) 
		{
			var indexedValue = propertyValue as IndexedCollection;
			if (indexedValue != null)
			{
				var idMatch = false;
				Property referencedProperty = null;
				PersistentClass refPc = null;
				if (referencedEntityName != null)
					refPc = mainGenerator.Cfg.GetClassMapping(referencedEntityName);

				if (refPc != null)
				{
					idMatch = MappingTools.SameColumns(refPc.IdentifierProperty.ColumnIterator, indexedValue.Index.ColumnIterator);
					foreach (var propertyRef in refPc.PropertyIterator)
					{
						if (MappingTools.SameColumns(propertyRef.ColumnIterator, indexedValue.Index.ColumnIterator))
						{
							referencedProperty = propertyRef;
							break;
						}
					}
				}
				if (!idMatch && referencedProperty == null)
				{
					return addValueToMiddleTable(indexedValue.Index, middleEntityXml,
							queryGeneratorBuilder, "mapkey", null);
				}
				var currentIndex = queryGeneratorBuilder == null ? 0 : queryGeneratorBuilder.CurrentIndex;
				if (idMatch)
				{
					// The key of the map is the id of the entity.
					var referencedIdMapping = mainGenerator.EntitiesConfigurations[referencedEntityName].IdMappingData;
					return new MiddleComponentData(new MiddleMapKeyIdComponentMapper(mainGenerator.VerEntCfg,
																					 referencedIdMapping.IdMapper), currentIndex);
				}
				if(indexedValue is Map)
				{
					// The key of the map is a property of the entity.
					return new MiddleComponentData(new MiddleMapKeyPropertyComponentMapper(referencedProperty.Name,
																						   referencedProperty.PropertyAccessorName), currentIndex);					
				}
				//bidirectional list
				// The key of the map is a property of the entity.
				return new MiddleComponentData(new MiddleStraightComponentMapper(referencedProperty.Name), currentIndex);
			}
			// No index - creating a dummy mapper.
			return new MiddleComponentData(new MiddleDummyComponentMapper(), 0);
		}

		/// <summary>
		/// Add value to middle table
		/// </summary>
		/// <param name="value">Value, which should be mapped to the middle-table, either as a relation to another entity, or as a simple value.</param>
		/// <param name="xmlMapping">If not <code>null</code>, xml mapping for this value is added to this element.</param>
		/// <param name="queryGeneratorBuilder">In case <code>value</code> is a relation to another entity, information about it should be added to the given.</param>
		/// <param name="prefix">Prefix for proeprty names of related entities identifiers.</param>
		/// <param name="joinColumns">Names of columns to use in the xml mapping, if this array isn't null and has any elements.</param>
		/// <returns>Data for mapping this component.</returns>
		private MiddleComponentData addValueToMiddleTable(IValue value, XmlElement xmlMapping,
														  QueryGeneratorBuilder queryGeneratorBuilder,
														  string prefix, string[] joinColumns) 
		{
			var type = value.Type;
			if (type is ManyToOneType) 
			{
				var prefixRelated = prefix + "_";
				var referencedEntityName = MappingTools.ReferencedEntityName(value);

				var referencedIdMapping = mainGenerator.GetReferencedIdMappingData(referencingEntityName,
						referencedEntityName, propertyAuditingData, true);

				// Adding related-entity (in this case: the referenced entities id) id mapping to the xml only if the
				// relation isn't inverse (so when <code>xmlMapping</code> is not null).
				if (xmlMapping != null) 
				{
					addRelatedToXmlMapping(xmlMapping, prefixRelated,
							joinColumns != null && joinColumns.Length > 0
									? joinColumns.ToList().GetEnumerator()
									: MetadataTools.GetColumnNameEnumerator(value.ColumnIterator),
							referencedIdMapping);
				}

				// Storing the id data of the referenced entity: original mapper, prefixed mapper and entity name.
				var referencedIdData = CreateMiddleIdData(referencedIdMapping,
						prefixRelated, referencedEntityName);
				// And adding it to the generator builder.
				queryGeneratorBuilder.AddRelation(referencedIdData);

				return new MiddleComponentData(new MiddleRelatedComponentMapper(referencedIdData),
						queryGeneratorBuilder.CurrentIndex);
			}
			// Last but one parameter: collection components are always insertable
			var mapped = mainGenerator.BasicMetadataGenerator.AddBasic(xmlMapping,
																		new PropertyAuditingData(prefix, "field", ModificationStore.Full, RelationTargetAuditMode.Audited, null, null, false),
																		value, null, true, true);

			if (mapped) 
			{
				// Simple values are always stored in the first item of the array returned by the query generator.
				return new MiddleComponentData(new MiddleSimpleComponentMapper(mainGenerator.VerEntCfg, prefix), 0);
			}
			mainGenerator.ThrowUnsupportedTypeException(type, referencingEntityName, propertyName);
			// Impossible to get here.
			throw new AssertionFailure();
		}

		private void AddMapper(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData,
							   MiddleComponentData indexComponentData)
		{
			var type = propertyValue.Type;
			var isGenericType = type.ReturnedClass.IsGenericType;

			IPropertyMapper collectionMapper;
			ICollectionProxyMapperFactory collectionProxyMapperFactory = new DefaultCollectionProxyMapperFactory();

			if (type is SetType)
			{
				if (isGenericType)
				{
					var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionProxyMapperFactory>("Set", 
																										type.ReturnedClass.GetGenericArguments(), 
																										new[] { typeof(CommonCollectionMapperData), typeof(MiddleComponentData) });
					collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
																							 new object[] { commonCollectionMapperData, elementComponentData });					
				}
				else
				{
					collectionMapper = collectionProxyMapperFactory.Set(commonCollectionMapperData, elementComponentData);
				}
			}
			else if (type is ListType)
			{
				if (isGenericType)
				{
					var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionProxyMapperFactory>("List", 
																										type.ReturnedClass.GetGenericArguments(), 
																										new[] { typeof(CommonCollectionMapperData), typeof(MiddleComponentData), typeof(MiddleComponentData) });
					collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
																							 new object[] { commonCollectionMapperData, elementComponentData, indexComponentData });
				}
				else
				{
					collectionMapper = collectionProxyMapperFactory.List(commonCollectionMapperData, elementComponentData, indexComponentData);
				}
			}
			else if (type is MapType)
			{
				if (isGenericType)
				{
					var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionProxyMapperFactory>("Map",
																					type.ReturnedClass.GetGenericArguments(),
																					new[] { typeof(CommonCollectionMapperData), typeof(MiddleComponentData), typeof(MiddleComponentData) });
					collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
																		 new object[] { commonCollectionMapperData, elementComponentData, indexComponentData });
				}
				else
				{
					collectionMapper = collectionProxyMapperFactory.Map(commonCollectionMapperData, elementComponentData, indexComponentData);
				}
			}
			else if (type is BagType)
			{
				if (isGenericType)
				{
					var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionProxyMapperFactory>("Bag",
																										type.ReturnedClass.GetGenericArguments(),
																										new[] { typeof(CommonCollectionMapperData), typeof(MiddleComponentData) });
					collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
																							 new object[] { commonCollectionMapperData, elementComponentData });
				}
				else
				{
					collectionMapper = collectionProxyMapperFactory.Bag(commonCollectionMapperData, elementComponentData);
				}
			
			}
			else
				throw new NotImplementedException();
			/*
			 * 
			 *         if (type instanceof SortedSetType) {
			currentMapper.addComposite(propertyAuditingData.getPropertyData(),
					new BasicCollectionMapper<Set>(commonCollectionMapperData,
					TreeSet.class, SortedSetProxy.class, elementComponentData));
		} else if (type instanceof SetType) {
			currentMapper.addComposite(propertyAuditingData.getPropertyData(),
					new BasicCollectionMapper<Set>(commonCollectionMapperData,
					HashSet.class, SetProxy.class, elementComponentData));
		} else if (type instanceof SortedMapType) {
			// Indexed collection, so <code>indexComponentData</code> is not null.
			currentMapper.addComposite(propertyAuditingData.getPropertyData(),
					new MapCollectionMapper<Map>(commonCollectionMapperData,
					TreeMap.class, SortedMapProxy.class, elementComponentData, indexComponentData));
		} else if (type instanceof MapType) {
			// Indexed collection, so <code>indexComponentData</code> is not null.
			currentMapper.addComposite(propertyAuditingData.getPropertyData(),
					new MapCollectionMapper<Map>(commonCollectionMapperData,
					HashMap.class, MapProxy.class, elementComponentData, indexComponentData));
		} else if (type instanceof BagType) {
			currentMapper.addComposite(propertyAuditingData.getPropertyData(),
					new BasicCollectionMapper<List>(commonCollectionMapperData,
					ArrayList.class, ListProxy.class, elementComponentData));
		} else if (type instanceof ListType) {
			// Indexed collection, so <code>indexComponentData</code> is not null.
			currentMapper.addComposite(propertyAuditingData.getPropertyData(),
					new ListCollectionMapper(commonCollectionMapperData,
					elementComponentData, indexComponentData));
			 */

			currentMapper.AddComposite(propertyAuditingData.GetPropertyData(), collectionMapper);
		}

		private void StoreMiddleEntityRelationInformation(string mappedBy) 
		{
			// Only if this is a relation (when there is a referenced entity).
			if (referencedEntityName != null)
			{
				if (propertyValue.IsInverse)
				{
					referencingEntityConfiguration.AddToManyMiddleNotOwningRelation(propertyName, mappedBy, referencedEntityName);
				}
				else
				{
					referencingEntityConfiguration.addToManyMiddleRelation(propertyName, referencedEntityName);
				}
			}
		}

		private XmlElement CreateMiddleEntityXml(string auditMiddleTableName, string auditMiddleEntityName, string where) 
		{
			var schema = mainGenerator.GetSchema(propertyAuditingData.JoinTable.Schema, propertyValue.CollectionTable);
			var catalog = mainGenerator.GetCatalog(propertyAuditingData.JoinTable.Catalog, propertyValue.CollectionTable);

			var middleEntityXml = MetadataTools.CreateEntity(xmlMappingData.NewAdditionalMapping(),
					new AuditTableData(auditMiddleEntityName, auditMiddleTableName, schema, catalog), null);
			var middleEntityXmlId = middleEntityXml.OwnerDocument.CreateElement("composite-id");
			middleEntityXml.AppendChild(middleEntityXmlId);

			// If there is a where clause on the relation, adding it to the middle entity.
			if (where != null)
			{
				middleEntityXml.SetAttribute("where", where);
			}

			middleEntityXmlId.SetAttribute("name", mainGenerator.VerEntCfg.OriginalIdPropName);

			// Adding the revision number as a foreign key to the revision info entity to the composite id of the
			// middle table.
			mainGenerator.AddRevisionInfoRelation(middleEntityXmlId);

			// Adding the revision type property to the entity xml.
			mainGenerator.AddRevisionType(middleEntityXml);

			// All other properties should also be part of the primary key of the middle entity.
			return middleEntityXmlId;
		}

		private string GetMappedBy(Mapping.Collection collectionValue) 
		{
			var referencedClass = ((OneToMany)collectionValue.Element).AssociatedClass;

			// If there's an @AuditMappedBy specified, returning it directly.
			var auditMappedBy = propertyAuditingData.AuditMappedBy;
			if (auditMappedBy != null)
			{
				return auditMappedBy;
			}

			foreach (var property in referencedClass.PropertyIterator)
			{
				if (Toolz.IteratorsContentEqual(property.Value.ColumnIterator.GetEnumerator(),
							collectionValue.Key.ColumnIterator.GetEnumerator()))
				{
					return property.Name;
				}
			}

			throw new MappingException("Unable to read the mapped by attribute for " + propertyName + " in "
					+ referencingEntityName + "!");
		}

		private string GetMappedBy(Table collectionTable, PersistentClass referencedClass) 
		{
			// If there's an @AuditMappedBy specified, returning it directly.
			var auditMappedBy = propertyAuditingData.AuditMappedBy;
			if (auditMappedBy != null)
			{
				return auditMappedBy;
			}

			foreach (var property in referencedClass.PropertyIterator)
			{
				var propValueAsColl = property.Value as Mapping.Collection;
				if (propValueAsColl != null)
				{
					// The equality is intentional. We want to find a collection property with the same collection table.
					if (propValueAsColl.CollectionTable == collectionTable)
					{
						return property.Name;
					}
				}
			}

			throw new MappingException("Unable to read the mapped by attribute for " + propertyName + " in "+ referencingEntityName + "!");
		}
	}
}
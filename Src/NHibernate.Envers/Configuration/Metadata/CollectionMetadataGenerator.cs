using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Configuration.Store;
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

		private readonly IMetaDataStore _metaDataStore;
		private readonly AuditMetadataGenerator _mainGenerator;
		private readonly string _propertyName;
		private readonly Mapping.Collection _propertyValue;
		private readonly ICompositeMapperBuilder _currentMapper;
		private readonly string _referencingEntityName;
		private readonly EntityXmlMappingData _xmlMappingData;
		private readonly PropertyAuditingData _propertyAuditingData;
		private readonly EntityConfiguration _referencingEntityConfiguration;

		/// <summary>
		/// Null if this collection isn't a relation to another entity.
		/// </summary>
		private readonly string _referencedEntityName;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="metaDataStore"></param>
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
		public CollectionMetadataGenerator(IMetaDataStore metaDataStore,
											AuditMetadataGenerator mainGenerator,
											Mapping.Collection propertyValue,
											ICompositeMapperBuilder currentMapper,
											string referencingEntityName,
											EntityXmlMappingData xmlMappingData,
											PropertyAuditingData propertyAuditingData)
		{
			_metaDataStore = metaDataStore;
			_mainGenerator = mainGenerator;
			_propertyValue = propertyValue;
			_currentMapper = currentMapper;
			_referencingEntityName = referencingEntityName;
			_xmlMappingData = xmlMappingData;
			_propertyAuditingData = propertyAuditingData;

			_propertyName = propertyAuditingData.Name;

			_referencingEntityConfiguration = mainGenerator.EntitiesConfigurations[referencingEntityName];
			if (_referencingEntityConfiguration == null)
			{
				throw new MappingException("Unable to read auditing configuration for " + referencingEntityName + "!");
			}

			_referencedEntityName = MappingTools.ReferencedEntityName(propertyValue.Element);
		}

		public void AddCollection()
		{
			var type = _propertyValue.Type;
			var value = _propertyValue.Element;

			var oneToManyAttachedType = type.IsCollectionType;
			var inverseOneToMany = (value is OneToMany) && (_propertyValue.IsInverse);
			var owningManyToOneWithJoinTableBidirectional = value is ManyToOne && _propertyAuditingData.MappedBy != null;
			var fakeOneToManyBidirectional = (value is OneToMany) && (_propertyAuditingData.MappedBy != null);

			if (oneToManyAttachedType && (inverseOneToMany || fakeOneToManyBidirectional || owningManyToOneWithJoinTableBidirectional))
			{
				// A one-to-many relation mapped using @ManyToOne and @OneToMany(mappedBy="...")
				addOneToManyAttached(fakeOneToManyBidirectional);
			}
			else
			{
				// All other kinds of relations require a middle (join) table.
				addWithMiddleTable();
			}
		}

		private MiddleIdData createMiddleIdData(IdMappingData idMappingData, string prefix, string entityName)
		{
			return new MiddleIdData(_mainGenerator.VerEntCfg, idMappingData, prefix, entityName,
				_mainGenerator.EntitiesConfigurations.ContainsKey(entityName));
		}

		private void addOneToManyAttached(bool fakeOneToManyBidirectional)
		{
			log.DebugFormat("Adding audit mapping for property {0}. {1}" +
					": one-to-many collection, using a join column on the referenced entity.", _referencingEntityName, _propertyName);

			var mappedBy = getMappedBy(_propertyValue);

			var referencedIdMapping = _mainGenerator.GetReferencedIdMappingData(_referencingEntityName,
						_referencedEntityName, _propertyAuditingData, false);
			var referencingIdMapping = _referencingEntityConfiguration.IdMappingData;

			// Generating the id mappers data for the referencing side of the relation.
			var referencingIdData = createMiddleIdData(referencingIdMapping,
					mappedBy + "_", _referencingEntityName);

			// And for the referenced side. The prefixed mapper won't be used (as this collection isn't persisted
			// in a join table, so the prefix value is arbitrary).
			var referencedIdData = createMiddleIdData(referencedIdMapping,
					null, _referencedEntityName);

			// Generating the element mapping.
			var elementComponentData = new MiddleComponentData(
					new MiddleRelatedComponentMapper(referencedIdData), 0);

			// Generating the index mapping, if an index exists. It can only exists in case a javax.persistence.MapKey
			// annotation is present on the entity. So the middleEntityXml will be not be used. The queryGeneratorBuilder
			// will only be checked for nullnes.
			var indexComponentData = addIndex(null, null);

			// Generating the query generator - it should read directly from the related entity.
			var queryGenerator = new OneAuditEntityQueryGenerator(_mainGenerator.VerEntCfg,
					_mainGenerator.GlobalCfg.AuditStrategy, referencingIdData, _referencedEntityName, referencedIdData, isEmbeddableElementType());

			// Creating common mapper data.
			var commonCollectionMapperData = new CommonCollectionMapperData(
					_mainGenerator.VerEntCfg, _referencedEntityName,
					_propertyAuditingData.GetPropertyData(),
					referencingIdData, queryGenerator);

			IPropertyMapper fakeBidirectionalRelationMapper;
			IPropertyMapper fakeBidirectionalRelationIndexMapper;
			if (fakeOneToManyBidirectional)
			{
				// In case of a fake many-to-one bidirectional relation, we have to generate a mapper which maps
				// the mapped-by property name to the id of the related entity (which is the owner of the collection).
				var auditMappedBy = _propertyAuditingData.MappedBy;

				// Creating a prefixed relation mapper.
				var relMapper = referencingIdMapping.IdMapper.PrefixMappedProperties(
						MappingTools.CreateToOneRelationPrefix(auditMappedBy));

				fakeBidirectionalRelationMapper = new ToOneIdMapper(
						_mainGenerator.GlobalCfg.EnversProxyFactory,
						relMapper,
					// The mapper will only be used to map from entity to map, so no need to provide other details
					// when constructing the PropertyData.
						new PropertyData(auditMappedBy, null, null),
						_referencedEntityName, false);

				// Checking if there's an index defined. If so, adding a mapper for it.
				if (_propertyAuditingData.PositionMappedBy != null)
				{
					var positionMappedBy = _propertyAuditingData.PositionMappedBy;
					fakeBidirectionalRelationIndexMapper = new SinglePropertyMapper(new PropertyData(positionMappedBy, null, null));

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
			addMapper(commonCollectionMapperData, elementComponentData, indexComponentData);

			// Storing information about this relation.
			_referencingEntityConfiguration.AddToManyNotOwningRelation(_propertyName, mappedBy,
					_referencedEntityName, referencingIdData.PrefixedMapper, fakeBidirectionalRelationMapper,
					fakeBidirectionalRelationIndexMapper);
		}

		/// <summary>
		/// Adds mapping of the id of a related entity to the given xml mapping, prefixing the id with the given prefix.
		/// </summary>
		/// <param name="xmlMapping">Mapping, to which to add the xml.</param>
		/// <param name="prefix">Prefix for the names of properties which will be prepended to properties that form the id.</param>
		/// <param name="columnNames">Column names that will be used for properties that form the id.</param>
		/// <param name="relatedIdMapping">Id mapping data of the related entity.</param>
		private static void addRelatedToXmlMapping(XElement xmlMapping, string prefix,
											IEnumerable<string> columnNames,
											IdMappingData relatedIdMapping)
		{
			var properties = new XElement(relatedIdMapping.XmlRelationMapping);
			MetadataTools.PrefixNamesInPropertyElement(properties, prefix, columnNames, true, true);
			foreach (var idProperty in properties.Elements())
			{
				xmlMapping.Add(idProperty);
			}
		}

		private void addWithMiddleTable()
		{
			log.DebugFormat("Adding audit mapping for property {0}. {1}" +
					": collection with a join table.", _referencingEntityName, _propertyName);

			// Generating the name of the middle table
			string auditMiddleTableName;
			string auditMiddleEntityName;
			if (!string.IsNullOrEmpty(_propertyAuditingData.JoinTable.TableName))
			{
				auditMiddleTableName = _propertyAuditingData.JoinTable.TableName;
				auditMiddleEntityName = _propertyAuditingData.JoinTable.TableName;
			}
			else
			{
				// We check how Hibernate maps the collection.
				if (_propertyValue.Element is OneToMany && !_propertyValue.IsInverse)
				{
					// This must be a @JoinColumn+@OneToMany mapping. Generating the table name, as Hibernate doesn't use a
					// middle table for mapping this relation.
					var referencingPersistentClass = _mainGenerator.Cfg.GetClassMapping(_referencingEntityName);
					var referencedPersistentClass = _mainGenerator.Cfg.GetClassMapping(_referencedEntityName);
					auditMiddleTableName = _mainGenerator.VerEntCfg.UnidirectionOneToManyTableName(referencingPersistentClass, referencedPersistentClass);
					auditMiddleEntityName = _mainGenerator.VerEntCfg.GetAuditEntityName(_referencingEntityName, _referencedEntityName);
				}
				else
				{
					auditMiddleTableName = _mainGenerator.VerEntCfg.CollectionTableName(_propertyValue);
					auditMiddleEntityName = _mainGenerator.VerEntCfg.GetAuditEntityName(_propertyValue.CollectionTable.Name);
				}
			}

			log.DebugFormat("Using join table name: {0}", auditMiddleTableName);

			// Generating the XML mapping for the middle entity, only if the relation isn't inverse.
			// If the relation is inverse, will be later checked by comparing middleEntityXml with null.
			XElement middleEntityXml;
			if (!_propertyValue.IsInverse)
			{
				// Generating a unique middle entity name
				auditMiddleEntityName = _mainGenerator.AuditEntityNameRegister.CreateUnique(auditMiddleEntityName);

				// Registering the generated name
				_mainGenerator.AuditEntityNameRegister.Register(auditMiddleEntityName);

				middleEntityXml = createMiddleEntityXml(auditMiddleTableName, auditMiddleEntityName, _propertyValue.Where);
			}
			else
			{
				middleEntityXml = null;
			}

			// ******
			// Generating the mapping for the referencing entity (it must be an entity).
			// ******
			// Getting the id-mapping data of the referencing entity (the entity that "owns" this collection).
			var referencingIdMapping = _referencingEntityConfiguration.IdMappingData;

			// Only valid for an inverse relation; null otherwise.
			string mappedBy;

			// The referencing prefix is always for a related entity. So it has always the "_" at the end added.
			string referencingPrefixRelated;
			string referencedPrefix;

			if (_propertyValue.IsInverse)
			{
				// If the relation is inverse, then referencedEntityName is not null.
				mappedBy = getMappedBy(_propertyValue.CollectionTable, _mainGenerator.Cfg.GetClassMapping(_referencedEntityName));

				referencingPrefixRelated = mappedBy + "_";
				referencedPrefix = StringTools.GetLastComponent(_referencedEntityName);
			}
			else
			{
				mappedBy = null;

				referencingPrefixRelated = StringTools.GetLastComponent(_referencingEntityName) + "_";
				referencedPrefix = _referencedEntityName == null ? "element" : _propertyName;
			}

			// Storing the id data of the referencing entity: original mapper, prefixed mapper and entity name.
			var referencingIdData = createMiddleIdData(referencingIdMapping, referencingPrefixRelated, _referencingEntityName);

			// Creating a query generator builder, to which additional id data will be added, in case this collection
			// references some entities (either from the element or index). At the end, this will be used to build
			// a query generator to read the raw data collection from the middle table.
			var queryGeneratorBuilder = new QueryGeneratorBuilder(_mainGenerator.VerEntCfg, _mainGenerator.GlobalCfg.AuditStrategy, referencingIdData, auditMiddleEntityName, isEmbeddableElementType());

			// Adding the XML mapping for the referencing entity, if the relation isn't inverse.
			if (middleEntityXml != null)
			{
				// Adding related-entity (in this case: the referencing's entity id) id mapping to the xml.
				addRelatedToXmlMapping(middleEntityXml, referencingPrefixRelated,
						MetadataTools.GetColumnNameEnumerator(_propertyValue.Key.ColumnIterator),
						referencingIdMapping);
			}

			// ******
			// Generating the element mapping.
			// ******
			var elementComponentData = addValueToMiddleTable(_propertyValue.Element, middleEntityXml,
					queryGeneratorBuilder, referencedPrefix, _propertyAuditingData.JoinTable.InverseJoinColumns);

			// ******
			// Generating the index mapping, if an index exists.
			// ******
			var indexComponentData = addIndex(middleEntityXml, queryGeneratorBuilder);

			// ******
			// Generating the property mapper.
			// ******
			// Building the query generator.
			var queryGenerator = queryGeneratorBuilder.Build(new Collection<MiddleComponentData> { elementComponentData, indexComponentData });

			// Creating common data
			var commonCollectionMapperData = new CommonCollectionMapperData(
					_mainGenerator.VerEntCfg, auditMiddleEntityName,
					_propertyAuditingData.GetPropertyData(),
					referencingIdData, queryGenerator);

			// Checking the type of the collection and adding an appropriate mapper.
			addMapper(commonCollectionMapperData, elementComponentData, indexComponentData);

			// ******
			// Storing information about this relation.
			// ******
			storeMiddleEntityRelationInformation(mappedBy);
		}

		private MiddleComponentData addIndex(XElement middleEntityXml, QueryGeneratorBuilder queryGeneratorBuilder)
		{
			var indexedValue = _propertyValue as IndexedCollection;
			if (indexedValue != null)
			{
				var idMatch = false;
				Property referencedProperty = null;
				PersistentClass refPc = null;
				if (_referencedEntityName != null)
					refPc = _mainGenerator.Cfg.GetClassMapping(_referencedEntityName);

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
					var referencedIdMapping = _mainGenerator.EntitiesConfigurations[_referencedEntityName].IdMappingData;
					return new MiddleComponentData(new MiddleMapKeyIdComponentMapper(_mainGenerator.VerEntCfg,
																					 referencedIdMapping.IdMapper), currentIndex);
				}
				if (indexedValue is Map)
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
		private MiddleComponentData addValueToMiddleTable(IValue value, XElement xmlMapping,
														  QueryGeneratorBuilder queryGeneratorBuilder,
														  string prefix, string[] joinColumns)
		{
			var type = value.Type;
			if (type is ManyToOneType)
			{
				var prefixRelated = prefix + "_";
				var referencedEntityName = MappingTools.ReferencedEntityName(value);

				var referencedIdMapping = _mainGenerator.GetReferencedIdMappingData(_referencingEntityName,
						referencedEntityName, _propertyAuditingData, true);

				// Adding related-entity (in this case: the referenced entities id) id mapping to the xml only if the
				// relation isn't inverse (so when <code>xmlMapping</code> is not null).
				if (xmlMapping != null)
				{
					addRelatedToXmlMapping(xmlMapping, prefixRelated,
							joinColumns != null && joinColumns.Length > 0
									? joinColumns
									: MetadataTools.GetColumnNameEnumerator(value.ColumnIterator),
							referencedIdMapping);
				}

				// Storing the id data of the referenced entity: original mapper, prefixed mapper and entity name.
				var referencedIdData = createMiddleIdData(referencedIdMapping,
						prefixRelated, referencedEntityName);
				// And adding it to the generator builder.
				queryGeneratorBuilder.AddRelation(referencedIdData);

				return new MiddleComponentData(new MiddleRelatedComponentMapper(referencedIdData),
						queryGeneratorBuilder.CurrentIndex);
			}
			else if (type is ComponentType)
			{
				//collection of embaddable elements
				var component = (Component)value;
				var componentMapper = new MiddleEmbeddableComponentMapper(new MultiPropertyMapper(), component.ComponentClassName);
				var parentXmlMapping = xmlMapping.Parent;

				var auditData = new ComponentAuditingData();

				new ComponentAuditedPropertiesReader(_metaDataStore,
																					new AuditedPropertiesReader.ComponentPropertiesSource(component),
																					auditData, _mainGenerator.GlobalCfg, "").Read();

				// Emulating first pass.
				foreach (var auditedPropertyName in auditData.PropertyNames)
				{
					var nestedAuditingData = auditData.GetPropertyAuditingData(auditedPropertyName);
					_mainGenerator.AddValue(parentXmlMapping, component.GetProperty(auditedPropertyName).Value, componentMapper, prefix, _xmlMappingData, nestedAuditingData, true, true, true);
				}

				// Emulating second pass so that the relations can be mapped too.
				foreach (var auditedPropertyName in auditData.PropertyNames)
				{
					var nestedAuditingData = auditData.GetPropertyAuditingData(auditedPropertyName);
					_mainGenerator.AddValue(parentXmlMapping, component.GetProperty(auditedPropertyName).Value, componentMapper, _referencingEntityName, _xmlMappingData, nestedAuditingData, true, false, true);
				}

				// Add an additional column holding a number to make each entry unique within the set.
				// Embeddable properties may contain null values, so cannot be stored within composite primary key.
				if (_propertyValue.IsSet)
				{
					var setOrdinalPropertyName = _mainGenerator.VerEntCfg.EmbeddableSetOrdinalPropertyName;
					var ordinalProperty = MetadataTools.AddProperty(xmlMapping, setOrdinalPropertyName, "int", true, true);
					MetadataTools.AddColumn(ordinalProperty, setOrdinalPropertyName, -1, -1, -1, null, false);
				}
				return new MiddleComponentData(componentMapper, 0);
			}
			// Last but one parameter: collection components are always insertable
			var mapped = _mainGenerator.BasicMetadataGenerator.AddBasic(xmlMapping,
																		new PropertyAuditingData(prefix, "field"),
																		value, null, true, true);

			if (mapped)
			{
				// Simple values are always stored in the first item of the array returned by the query generator.
				return new MiddleComponentData(new MiddleSimpleComponentMapper(_mainGenerator.VerEntCfg, prefix), 0);
			}
			_mainGenerator.ThrowUnsupportedTypeException(type, _referencingEntityName, _propertyName);
			// Impossible to get here.
			throw new AssertionFailure();
		}

		private System.Type genericTypeDefinition(IType type)
		{
			var clrType = type.GetType();
			return clrType.IsGenericType ? clrType.GetGenericTypeDefinition() : null;
		}

		private void addMapper(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData,
								MiddleComponentData indexComponentData)
		{
			var type = _propertyValue.Type;
			var embeddableElementType = isEmbeddableElementType();

			IPropertyMapper collectionMapper;
			var collectionProxyMapperFactory = _mainGenerator.GlobalCfg.CollectionMapperFactory;
			var genericType = genericTypeDefinition(type);

			if (_propertyAuditingData.CustomCollectionMapperFactory != null)
			{
				collectionMapper = _propertyAuditingData.CustomCollectionMapperFactory.Create(_mainGenerator.GlobalCfg.EnversProxyFactory, commonCollectionMapperData, elementComponentData, indexComponentData, embeddableElementType);
			}
			else if (genericType == typeof(GenericSortedSetType<>))
			{
				var comparerType = createGenericComparerType(type);
				var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionMapperFactory>("SortedSet",
					type.ReturnedClass.GetGenericArguments(),
						new[] { typeof(IEnversProxyFactory), typeof(CommonCollectionMapperData), typeof(MiddleComponentData), comparerType, typeof(bool) });
				collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
						new[] { _mainGenerator.GlobalCfg.EnversProxyFactory, commonCollectionMapperData, elementComponentData, _propertyValue.Comparer, embeddableElementType });
			}
			else if (genericType == typeof (GenericSetType<>))
			{
					var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionMapperFactory>("Set",
						type.ReturnedClass.GetGenericArguments(),
							new[] { typeof(IEnversProxyFactory), typeof(CommonCollectionMapperData), typeof(MiddleComponentData), typeof(bool) });
					collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
							new object[] { _mainGenerator.GlobalCfg.EnversProxyFactory, commonCollectionMapperData, elementComponentData, embeddableElementType });				
			}
			else if (genericType == typeof(GenericListType<>))
			{
				var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionMapperFactory>("List",
					type.ReturnedClass.GetGenericArguments(),
					new[] { typeof(IEnversProxyFactory), typeof(CommonCollectionMapperData), typeof(MiddleComponentData), typeof(MiddleComponentData), typeof(bool) });
				collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
					new object[] { _mainGenerator.GlobalCfg.EnversProxyFactory, commonCollectionMapperData, elementComponentData, indexComponentData, embeddableElementType });

			}
			else if (genericType == typeof(GenericSortedDictionaryType<,>))
			{
				var comparerType = createGenericComparerType(type);
				var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionMapperFactory>("SortedMap",
					type.ReturnedClass.GetGenericArguments(),
					new[] { typeof(IEnversProxyFactory), typeof(CommonCollectionMapperData), typeof(MiddleComponentData), typeof(MiddleComponentData), comparerType, typeof(bool) });
				collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
					new[] { _mainGenerator.GlobalCfg.EnversProxyFactory, commonCollectionMapperData, elementComponentData, indexComponentData, _propertyValue.Comparer, embeddableElementType });
			}
			else if (genericType == typeof (GenericMapType<,>))
			{
				var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionMapperFactory>("Map",
					type.ReturnedClass.GetGenericArguments(),
					new[] { typeof(IEnversProxyFactory), typeof(CommonCollectionMapperData), typeof(MiddleComponentData), typeof(MiddleComponentData), typeof(bool) });
				collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
					new object[] { _mainGenerator.GlobalCfg.EnversProxyFactory, commonCollectionMapperData, elementComponentData, indexComponentData, embeddableElementType });								
			}
			else if (genericType == typeof(GenericBagType<>))
			{
				var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionMapperFactory>("Bag",
					type.ReturnedClass.GetGenericArguments(),
					new[] { typeof(IEnversProxyFactory), typeof(CommonCollectionMapperData), typeof(MiddleComponentData), typeof(bool) });
				collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
					new object[] { _mainGenerator.GlobalCfg.EnversProxyFactory, commonCollectionMapperData, elementComponentData, embeddableElementType });
			}
			else if (genericType == typeof(GenericIdentifierBagType<>))
			{
				var methodInfo = ReflectHelper.GetGenericMethodFrom<ICollectionMapperFactory>("IdBag",
					type.ReturnedClass.GetGenericArguments(),
					new[] { typeof(IEnversProxyFactory), typeof(CommonCollectionMapperData), typeof(MiddleComponentData), typeof(bool) });
				collectionMapper = (IPropertyMapper)methodInfo.Invoke(collectionProxyMapperFactory,
					new object[] { _mainGenerator.GlobalCfg.EnversProxyFactory, commonCollectionMapperData, elementComponentData, embeddableElementType });
			}
			else
			{
				if (type is CustomCollectionType)
					throw new NotImplementedException("Your custom collection type " + type.Name + " needs a defined " + typeof(ICustomCollectionMapperFactory).Name);
				throw new NotImplementedException("Mapped collection type " + type.Name + " is not currently supported in Envers");
			}

			_currentMapper.AddComposite(_propertyAuditingData.GetPropertyData(), collectionMapper);
		}

		private static System.Type createGenericComparerType(IType type)
		{
			var genericArgs = type.ReturnedClass.GetGenericArguments();
			var theGenericArg = genericArgs[0];
			return typeof(IComparer<>).MakeGenericType(theGenericArg);
		}

		private void storeMiddleEntityRelationInformation(string mappedBy)
		{
			// Only if this is a relation (when there is a referenced entity).
			if (_referencedEntityName != null)
			{
				if (_propertyValue.IsInverse)
				{
					_referencingEntityConfiguration.AddToManyMiddleNotOwningRelation(_propertyName, mappedBy, _referencedEntityName);
				}
				else
				{
					_referencingEntityConfiguration.AddToManyMiddleRelation(_propertyName, _referencedEntityName);
				}
			}
		}

		private XElement createMiddleEntityXml(string auditMiddleTableName, string auditMiddleEntityName, string where)
		{
			var schema = _mainGenerator.GetSchema(_propertyAuditingData.JoinTable.Schema, _propertyValue.CollectionTable);
			var catalog = _mainGenerator.GetCatalog(_propertyAuditingData.JoinTable.Catalog, _propertyValue.CollectionTable);

			var middleEntityXml = MetadataTools.CreateEntity(_xmlMappingData.NewAdditionalMapping(),
					new AuditTableData(auditMiddleEntityName, auditMiddleTableName, schema, catalog), null, false);
			var middleEntityXmlId = new XElement(MetadataTools.CreateElementName("composite-id"),
				new XAttribute("name", _mainGenerator.VerEntCfg.OriginalIdPropName));
			middleEntityXml.Add(middleEntityXmlId);

			// If there is a where clause on the relation, adding it to the middle entity.
			if (where != null)
			{
				middleEntityXml.Add(new XAttribute("where", where));
			}

			// Adding the revision number as a foreign key to the revision info entity to the composite id of the
			// middle table.
			_mainGenerator.AddRevisionInfoRelation(middleEntityXmlId);

			// Adding the revision type property to the entity xml.
			_mainGenerator.AddRevisionType(isEmbeddableElementType() ? middleEntityXmlId : middleEntityXml, middleEntityXml);

			// All other properties should also be part of the primary key of the middle entity.
			return middleEntityXmlId;
		}

		private bool isEmbeddableElementType()
		{
			return _propertyValue.Element.Type is ComponentType;
		}

		private string getMappedBy(Mapping.Collection collectionValue)
		{
			var referencedClass = _mainGenerator.Cfg.GetClassMapping(MappingTools.ReferencedEntityName(collectionValue.Element));
			var mappedBy = searchMappedBy(referencedClass, collectionValue);

			if (mappedBy == null)
			{
				log.Debug("Going to search the mapped by attribute for " + _propertyName + " in superclasses of entity: " + referencedClass.ClassName);
				var tempClass = referencedClass;
				while (mappedBy == null && tempClass.Superclass != null)
				{
					log.Debug("Searching in superclass: " + tempClass.Superclass.ClassName);
					mappedBy = searchMappedBy(tempClass.Superclass, collectionValue);
					tempClass = tempClass.Superclass;
				}
			}
			if (mappedBy == null)
				throw new MappingException("Cannot find the inverse side for " + _propertyName + " in " + _referencingEntityName + "!");
			return mappedBy;
		}

		private string getMappedBy(Table collectionTable, PersistentClass referencedClass)
		{
			// If there's an @AuditMappedBy specified, returning it directly.
			//var auditMappedBy = _propertyAuditingData.AuditMappedBy;
			//if (auditMappedBy != null)
			//{
			//   return auditMappedBy;
			//}

			var mappedBy = searchMappedBy(referencedClass, collectionTable);

			// not found on referenced class, searching on superclasses
			if (mappedBy == null)
			{
				log.Debug("Going to search the mapped by attribute for " + _propertyName + " in superclases of entity: " + referencedClass.ClassName);
				var tempClass = referencedClass;
				while (mappedBy == null && tempClass.Superclass != null)
				{
					log.Debug("Searching in superclass: " + tempClass.Superclass.ClassName);
					mappedBy = searchMappedBy(tempClass.Superclass, collectionTable);
					tempClass = tempClass.Superclass;
				}
			}

			if (mappedBy == null)
				throw new MappingException("Unable to read the mapped by attribute for " + _propertyName + " in " + _referencingEntityName + "!");
			return mappedBy;
		}

		private static string searchMappedBy(PersistentClass referencedClass, Mapping.Collection collectionValue)
		{
			foreach (var property in referencedClass.PropertyIterator)
			{
				//should probably not care if order is same...
				if (property.Value.ColumnIterator.SequenceEqual(collectionValue.Key.ColumnIterator))
				{
					return property.Name;
				}
			}
			return null;
		}

		private static string searchMappedBy(PersistentClass referencedClass, Table collectionTable)
		{
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
			return null;
		}
	}
}
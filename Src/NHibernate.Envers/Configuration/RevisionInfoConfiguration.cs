using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Entities;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Mapping;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Envers.Configuration
{
	public class RevisionInfoConfiguration
	{
		private readonly GlobalConfiguration _globalCfg;
		private readonly IMetaDataStore _metaDataStore;
		private string revisionInfoEntityName;
		private PropertyData revisionInfoIdData;
		private PropertyData revisionInfoTimestampData;
		private PropertyData modifiedEntityTypesData;
		private IType revisionInfoTimestampType;
		private System.Type revisionPropType;
		private string revisionPropSqlType;
		private string revisionAssQName;

		public RevisionInfoConfiguration(GlobalConfiguration globalCfg, IMetaDataStore metaDataStore)
		{
			_globalCfg = globalCfg;
			_metaDataStore = metaDataStore;
			revisionInfoEntityName = "NHibernate.Envers.DefaultRevisionEntity";
			revisionInfoIdData = new PropertyData("Id", "Id", "property", ModificationStore.None);
			revisionInfoTimestampData = new PropertyData("RevisionDate", "RevisionDate", "property", ModificationStore.None);
			modifiedEntityTypesData = new PropertyData("ModifiedEntityTypes", "ModifiedEntityTypes", "property", ModificationStore.None);
			revisionInfoTimestampType = new TimestampType(); //ORIG: LongType();
			revisionPropType = typeof(int);
		}

		private XmlDocument generateDefaultRevisionInfoXmlMapping()
		{
			var document = new XmlDocument();

			var classMapping = MetadataTools.CreateEntity(document,
									new AuditTableData(null, null, _globalCfg.DefaultSchemaName, _globalCfg.DefaultCatalogName),
									null);

			classMapping.SetAttribute("name", revisionInfoEntityName);
			classMapping.SetAttribute("table", "REVINFO");

			var idProperty = MetadataTools.AddNativelyGeneratedId(document, classMapping, revisionInfoIdData.Name, revisionPropType);
			//ORIG: MetadataTools.addColumn(idProperty, "REV", -1, 0, 0, null);
			var col = idProperty.OwnerDocument.CreateElement("column");
			col.SetAttribute("name", "REV");
			//idProperty should have a "generator" node otherwise sth. is wrong.
			idProperty.InsertBefore(col, idProperty.GetElementsByTagName("generator")[0]);

			var timestampProperty = MetadataTools.AddProperty(classMapping, revisionInfoTimestampData.Name,
					revisionInfoTimestampType.Name, true, false);
			MetadataTools.AddColumn(timestampProperty, "REVTSTMP", -1, 0, 0, SqlTypeFactory.DateTime.ToString());

			if (_globalCfg.IsTrackEntitiesChangedInRevisionEnabled)
			{
				generateEntityTypesTrackingTableMapping(classMapping, "ModifiedEntityTypes", "REVCHANGES", "REV", "ENTITYTYPE", "string");
			}

			return document;
		}

		private void generateEntityTypesTrackingTableMapping(XmlElement classMapping, string propertyName,
																				string joinTableName, string joinTablePrimaryKeyColumnName,
																				string joinTableValueColumnName, string joinTableValueColumnType)
		{
			var set = classMapping.OwnerDocument.CreateElement("set");
			classMapping.AppendChild(set);
			set.SetAttribute("name", propertyName);
			set.SetAttribute("table", joinTableName);
			set.SetAttribute("cascade", "persist, delete");
			set.SetAttribute("fetch", "join");
			set.SetAttribute("lazy", "false");
			var key = set.OwnerDocument.CreateElement("key");
			set.AppendChild(key);
			key.SetAttribute("column", joinTablePrimaryKeyColumnName);
			var element = set.OwnerDocument.CreateElement("element");
			set.AppendChild(element);
			element.SetAttribute("type", joinTableValueColumnType);
			var column = element.OwnerDocument.CreateElement("column");
			element.AppendChild(column);
			column.SetAttribute("name", joinTableValueColumnName);
		}

		private XmlElement generateRevisionInfoRelationMapping()
		{
			var document = new XmlDocument();
			var revRelMapping = document.CreateElement("key-many-to-one");
			revRelMapping.SetAttribute("class", revisionAssQName);

			if (revisionPropSqlType != null)
			{
				// Putting a fake name to make Hibernate happy. It will be replaced later anyway.
				MetadataTools.AddColumn(revRelMapping, "*", -1, 0, 0, revisionPropSqlType);
			}

			return revRelMapping;
		}

		private bool searchForTimestampCfg(IEnumerable<DeclaredPersistentProperty> persistentProperties)
		{
			var revisionTimestampFound = false;
			foreach (var persistentProperty in persistentProperties)
			{
				var member = persistentProperty.Member;
				var property = persistentProperty.Property;
				var revisionTimestamp = _metaDataStore.MemberMeta<RevisionTimestampAttribute>(member);
				if (revisionTimestamp != null)
				{
					if (revisionTimestampFound)
					{
						throw new MappingException("Only one property may be decorated with [RevisionTimestampAttribute]!");
					}

					var revisionTimestampType = property.Type.ReturnedClass;
					if (typeof(DateTime).Equals(revisionTimestampType) ||
							typeof(long).Equals(revisionTimestampType))
					{
						revisionInfoTimestampData = new PropertyData(property.Name, property.Name, property.PropertyAccessorName, ModificationStore.None);
						revisionTimestampFound = true;
					}
					else
					{
						throw new MappingException("The field decorated with @RevisionTimestamp must be of type DateTime or long");
					}
				}
			}
			return revisionTimestampFound;
		}

		private bool searchForEntityNamesCfg(IEnumerable<DeclaredPersistentProperty> persistentProperties)
		{
			var found = false;
			foreach (var persistentProperty in persistentProperties)
			{
				var member = persistentProperty.Member;
				var property = persistentProperty.Property;
				var entityName = _metaDataStore.MemberMeta<ModifiedEntityTypesAttribute>(member);
				if (entityName != null)
				{
					if (found)
						throw new MappingException("Only one property may be annotated with ModifiedEntityTypesAttribute!");
					if (property.Type.ReturnedClass.Equals(typeof(ISet<string>)))
					{
						modifiedEntityTypesData = new PropertyData(property.Name, property.Name, property.PropertyAccessorName, ModificationStore.None);
						found = true;
					}
					else
					{
						throw new MappingException("The property annotated with ModifiedEntityTypesAttribute must be of ISet<string> type.");
					}
				}
			}
			return found;
		}

		private bool searchForRevisionNumberCfg(IEnumerable<DeclaredPersistentProperty> persistentProperties)
		{
			var revisionNumberFound = false;
			foreach (var persistentProperty in persistentProperties)
			{
				var member = persistentProperty.Member;
				var property = persistentProperty.Property;
				var revisionNumber = _metaDataStore.MemberMeta<RevisionNumberAttribute>(member);
				if (revisionNumber != null)
				{
					if (revisionNumberFound)
					{
						throw new MappingException("Only one property may have the attribute [RevisionNumber]!");
					}

					var revNrType = property.Type.ReturnedClass;
					if (revNrType.Equals(typeof(int)))
					{
						revisionInfoIdData = new PropertyData(property.Name, property.Name, property.PropertyAccessorName, ModificationStore.None);
						revisionNumberFound = true;
					}
					else if (revNrType.Equals(typeof(long)))
					{
						revisionInfoIdData = new PropertyData(property.Name, property.Name, property.PropertyAccessorName, ModificationStore.None);
						revisionNumberFound = true;

						// The default is integer
						revisionPropType = typeof(long);
					}
					else
					{
						throw new MappingException("The field decorated with [RevisionNumberAttribute] must be of type int or long");
					}

					// Getting the @Column definition of the revision number property, to later use that info to
					// generate the same mapping for the relation from an audit table's revision number to the
					// revision entity revision number.
					var revisionPropColumn = (Column)persistentProperty.Property.ColumnIterator.First();
					if (!string.IsNullOrEmpty(revisionPropColumn.SqlType))
					{
						revisionPropSqlType = revisionPropColumn.SqlType;
					}
				}

			}
			return revisionNumberFound;
		}

		public RevisionInfoConfigurationResult Configure(Cfg.Configuration cfg)
		{
			IRevisionInfoGenerator revisionInfoGenerator;
			XmlDocument revisionInfoXmlMapping = null;
			System.Type revisionInfoClass;

			var revEntityType = _metaDataStore.EntitiesDeclaredWith<RevisionEntityAttribute>();
			var noOfRevEntities = revEntityType.Count();

			switch (noOfRevEntities)
			{
				case 0:
					{
						if (_globalCfg.IsTrackEntitiesChangedInRevisionEnabled)
						{
							revisionInfoClass = typeof (DefaultTrackingModifiedTypesRevisionEntity);
							revisionInfoEntityName = revisionInfoClass.FullName;
							revisionInfoGenerator = new DefaultTrackingModifiedTypesRevisionInfoGenerator(revisionInfoEntityName,
																										revisionInfoClass,
																										null,
																										revisionInfoTimestampData,
																										isTimestampAsDate(),
																										modifiedEntityTypesData);
						}
						else
						{
							revisionInfoClass = typeof(DefaultRevisionEntity);
							revisionInfoGenerator = new DefaultRevisionInfoGenerator(revisionInfoEntityName, revisionInfoClass,
									null, revisionInfoTimestampData, isTimestampAsDate());							
						}
						revisionAssQName = revisionInfoClass.AssemblyQualifiedName;
						revisionInfoXmlMapping = generateDefaultRevisionInfoXmlMapping();
						break;
					}
				case 1:
					{
						var clazz = revEntityType.First();
						var revEntityAttr = _metaDataStore.ClassMeta<RevisionEntityAttribute>(clazz);
						var pc = cfg.GetClassMapping(clazz);
						// Checking if custom revision entity isn't audited))
						if (_metaDataStore.ClassMeta<AuditedAttribute>(clazz) != null)
						{
							throw new MappingException("An entity decorated with [RevisionEntity] cannot be audited!");
						}

						var propertiesPlusIdentifier = new List<Property>();
						propertiesPlusIdentifier.AddRange(pc.PropertyIterator);
						propertiesPlusIdentifier.Add(pc.IdentifierProperty);
						var persistentProperties = PropertyAndMemberInfo.PersistentInfo(clazz, propertiesPlusIdentifier);

						if (!searchForRevisionNumberCfg(persistentProperties))
						{
							throw new MappingException("An entity decorated with [RevisionEntity] must have a field decorated " +
														"with [RevisionNumber]!");
						}

						if (!searchForTimestampCfg(persistentProperties))
						{
							throw new MappingException("An entity decorated with [RevisionEntity] must have a field decorated " +
														"with [RevisionTimestamp]!");
						}

						var modifiedEntityTypesFound = searchForEntityNamesCfg(persistentProperties);

						revisionInfoEntityName = pc.EntityName;
						revisionAssQName = pc.MappedClass.AssemblyQualifiedName;

						revisionInfoClass = pc.MappedClass;
						revisionInfoTimestampType = pc.GetProperty(revisionInfoTimestampData.Name).Type;

						if (_globalCfg.IsTrackEntitiesChangedInRevisionEnabled ||
								modifiedEntityTypesFound ||
								typeof(DefaultTrackingModifiedTypesRevisionEntity).IsAssignableFrom(revisionInfoClass))
						{
							// If tracking modified entities parameter is enabled, custom revision info entity is a subtype
							// of DefaultTrackingModifiedTypesRevisionEntity class, or @ModifiedEntityTypes annotation is used.
							revisionInfoGenerator = new DefaultTrackingModifiedTypesRevisionInfoGenerator(revisionInfoEntityName,
							                                                                              revisionInfoClass,
							                                                                              revEntityAttr.Listener,
							                                                                              revisionInfoTimestampData,
							                                                                              isTimestampAsDate(),
							                                                                              modifiedEntityTypesData);
							_globalCfg.SetTrackEntitiesChangedInRevisionEnabled();
						}
						else
						{
							revisionInfoGenerator = new DefaultRevisionInfoGenerator(revisionInfoEntityName, revisionInfoClass,
															 revEntityAttr.Listener, revisionInfoTimestampData, isTimestampAsDate());	
						}
						break;
					}
				default:
					{
						throw new MappingException("Only one entity may be decorated with [RevisionEntity]!");
					}
			}

			return new RevisionInfoConfigurationResult(
					revisionInfoGenerator, 
					revisionInfoXmlMapping,
					new RevisionInfoQueryCreator(revisionInfoEntityName, revisionInfoIdData.Name, revisionInfoTimestampData.Name, isTimestampAsDate(), revisionPropType),
					generateRevisionInfoRelationMapping(),
					new RevisionInfoNumberReader(revisionInfoClass, revisionInfoIdData), 
					_globalCfg.IsTrackEntitiesChangedInRevisionEnabled ? new ModifiedEntityTypesReader(revisionInfoClass, modifiedEntityTypesData) : null,
					revisionInfoEntityName, 
					revisionInfoClass, 
					revisionInfoTimestampData);
		}

		private bool isTimestampAsDate()
		{
			var type = revisionInfoTimestampType.ReturnedClass;
			return type.Equals(typeof(DateTime));
		}
	}
}

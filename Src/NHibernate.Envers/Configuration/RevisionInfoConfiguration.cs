using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Configuration.Metadata;
using System.Xml;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.SqlTypes;

namespace NHibernate.Envers.Configuration
{
	public class RevisionInfoConfiguration 
	{
		private readonly PropertyAndMemberInfo _propertyAndMemberInfo;
		private string revisionInfoEntityName;
		private PropertyData revisionInfoIdData;
		private PropertyData revisionInfoTimestampData;
		private IType revisionInfoTimestampType;
		private string revisionPropType;
		private string revisionPropSqlType;
		private string revisionAssQName;

		public RevisionInfoConfiguration(PropertyAndMemberInfo propertyAndMemberInfo) 
		{
			_propertyAndMemberInfo = propertyAndMemberInfo;
			revisionInfoEntityName = "NHibernate.Envers.DefaultRevisionEntity";
			revisionInfoIdData = new PropertyData("Id", "Id", "property", ModificationStore._NULL);
			revisionInfoTimestampData = new PropertyData("RevisionDate", "RevisionDate", "property", ModificationStore._NULL);
			revisionInfoTimestampType = new TimestampType(); //ORIG: LongType();

			revisionPropType = "integer";
		}

		private XmlDocument generateDefaultRevisionInfoXmlMapping() 
		{
			var document = new XmlDocument();//ORIG: DocumentHelper.createDocument();

			var class_mapping = MetadataTools.CreateEntity(document, new AuditTableData(null, null, null, null), null);

			class_mapping.SetAttribute("name", revisionInfoEntityName);
			class_mapping.SetAttribute("table", "REVINFO");

			var idProperty = MetadataTools.AddNativelyGeneratedId(document,class_mapping, revisionInfoIdData.Name,
					revisionPropType);
			//ORIG: MetadataTools.addColumn(idProperty, "REV", -1, 0, 0, null);
			var col = idProperty.OwnerDocument.CreateElement("column");
			col.SetAttribute("name", "REV");
			//idProperty should have a "generator" node otherwise sth. is wrong.
			idProperty.InsertBefore(col, idProperty.GetElementsByTagName("generator")[0]);

			var timestampProperty = MetadataTools.AddProperty(class_mapping, revisionInfoTimestampData.Name,
					revisionInfoTimestampType.Name, true, false);
			MetadataTools.AddColumn(timestampProperty, "REVTSTMP", -1, 0, 0, SqlTypeFactory.DateTime.ToString());

			return document;
		}

		private XmlElement generateRevisionInfoRelationMapping() 
		{
			var document = new XmlDocument();
			var rev_rel_mapping = document.CreateElement("key-many-to-one");
			//rk: removed type attribute from key-many-to-one
			//rev_rel_mapping.SetAttribute("type", revisionPropType);
			//rk: changed here
			rev_rel_mapping.SetAttribute("class", revisionAssQName);

			if (revisionPropSqlType != null) 
			{
				// Putting a fake name to make Hibernate happy. It will be replaced later anyway.
				MetadataTools.AddColumn(rev_rel_mapping, "*" , -1, 0, 0, revisionPropSqlType);
			}

			return rev_rel_mapping;
		}

		private bool searchForTimestampCfg(IEnumerable<DeclaredPersistentProperty> persistentProperties)
		{
			var revisionTimestampFound = false;
			foreach (var persistentProperty in persistentProperties)
			{
				var member = persistentProperty.Member;
				var property = persistentProperty.Property;
				var revisionTimestamp = (RevisionTimestampAttribute)Attribute.GetCustomAttribute(member, typeof(RevisionTimestampAttribute));
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
						revisionInfoTimestampData = new PropertyData(property.Name, property.Name, property.PropertyAccessorName, ModificationStore._NULL);
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

		private bool searchForRevisionNumberCfg(IEnumerable<DeclaredPersistentProperty> persistentProperties)
		{
			var revisionNumberFound = false;
			foreach (var persistentProperty in persistentProperties)
			{
				var member = persistentProperty.Member;
				var property = persistentProperty.Property;
				var revisionNumber = (RevisionNumberAttribute)Attribute.GetCustomAttribute(member, typeof(RevisionNumberAttribute));
				if (revisionNumber != null)
				{
					if (revisionNumberFound)
					{
						throw new MappingException("Only one property may have the attribute [RevisionNumber]!");
					}

					var revNrType = property.Type.ReturnedClass;
					if (revNrType.Equals(typeof(int)))
					{
						revisionInfoIdData = new PropertyData(property.Name, property.Name, property.PropertyAccessorName, ModificationStore._NULL);
						revisionNumberFound = true;
					}
					else if (revNrType.Equals(typeof(long)))
					{
						revisionInfoIdData = new PropertyData(property.Name, property.Name, property.PropertyAccessorName, ModificationStore._NULL);
						revisionNumberFound = true;

						// The default is integer
						revisionPropType = "long";
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
			var classes = cfg.ClassMappings;
			var revisionEntityFound = false;
			IRevisionInfoGenerator revisionInfoGenerator = null;

			System.Type revisionInfoClass = null;

			foreach (var pc in classes) 
			{
				System.Type clazz;
				try 
				{
					clazz = System.Type.GetType(pc.ClassName, true);
				} 
				catch (Exception e) 
				{
					throw new MappingException(e);
				}

				var revisionEntity =  (RevisionEntityAttribute)Attribute.GetCustomAttribute(clazz, typeof(RevisionEntityAttribute));
				if (revisionEntity != null) 
				{
					if (revisionEntityFound) 
					{
						throw new MappingException("Only one entity may be decorated with [RevisionEntity]!");
					}

					// Checking if custom revision entity isn't audited
					if (Attribute.GetCustomAttribute(clazz, typeof(AuditedAttribute)) != null) 
					{
						throw new MappingException("An entity decorated with [RevisionEntity] cannot be audited!");
					}

					revisionEntityFound = true;

					var propertiesPlusIdentifier = new List<Property>();
					propertiesPlusIdentifier.AddRange(pc.PropertyIterator);
					propertiesPlusIdentifier.Add(pc.IdentifierProperty);
					var persistentProperties = _propertyAndMemberInfo.GetPersistentInfo(clazz, propertiesPlusIdentifier);

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

					revisionInfoEntityName = pc.EntityName;
					revisionAssQName = pc.MappedClass.AssemblyQualifiedName;

					revisionInfoClass = pc.MappedClass;
					revisionInfoTimestampType = pc.GetProperty(revisionInfoTimestampData.Name).Type;
					revisionInfoGenerator = new DefaultRevisionInfoGenerator(revisionInfoEntityName, revisionInfoClass,
						revisionEntity.Value, revisionInfoTimestampData, isTimestampAsDate());
				}
			}

			// In case of a custom revision info generator, the mapping will be null.
			XmlDocument revisionInfoXmlMapping = null;

			if (revisionInfoGenerator == null) 
			{
				revisionInfoClass = typeof(DefaultRevisionEntity);
				revisionAssQName = revisionInfoClass.AssemblyQualifiedName;
				revisionInfoGenerator = new DefaultRevisionInfoGenerator(revisionInfoEntityName, revisionInfoClass,
						typeof(IRevisionListener),revisionInfoTimestampData, isTimestampAsDate());
				revisionInfoXmlMapping = generateDefaultRevisionInfoXmlMapping();
			}

		

			return new RevisionInfoConfigurationResult(
					revisionInfoGenerator, revisionInfoXmlMapping,
					new RevisionInfoQueryCreator(revisionInfoEntityName, revisionInfoIdData.Name,
							revisionInfoTimestampData.Name, isTimestampAsDate()),
					generateRevisionInfoRelationMapping(),
					new RevisionInfoNumberReader(revisionInfoClass, revisionInfoIdData), revisionInfoEntityName);
		}

		private bool isTimestampAsDate() 
		{
			var type = revisionInfoTimestampType.ReturnedClass;
			return type.Equals(typeof(DateTime));
		}
	}

	public class RevisionInfoConfigurationResult 
	{
		public RevisionInfoConfigurationResult(IRevisionInfoGenerator revisionInfoGenerator,
										XmlDocument revisionInfoXmlMapping, 
										RevisionInfoQueryCreator revisionInfoQueryCreator,
										XmlElement revisionInfoRelationMapping,
										RevisionInfoNumberReader revisionInfoNumberReader, 
										String revisionInfoEntityName) {
			RevisionInfoGenerator = revisionInfoGenerator;
			RevisionInfoXmlMapping = revisionInfoXmlMapping;
			RevisionInfoQueryCreator = revisionInfoQueryCreator;
			RevisionInfoRelationMapping = revisionInfoRelationMapping;
			RevisionInfoNumberReader = revisionInfoNumberReader;
			RevisionInfoEntityName = revisionInfoEntityName;
		}

		public IRevisionInfoGenerator RevisionInfoGenerator { get; private set; }
		public XmlDocument RevisionInfoXmlMapping { get; private set; }
		public RevisionInfoQueryCreator RevisionInfoQueryCreator { get; private set; }
		public XmlElement RevisionInfoRelationMapping { get; private set; }
		public RevisionInfoNumberReader RevisionInfoNumberReader { get; private set; }
		public String RevisionInfoEntityName { get; private set; }
	}
}

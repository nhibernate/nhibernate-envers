using System.Linq;
using System.Xml.Linq;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Mapping;
using NHibernate.Type;

namespace NHibernate.Envers.Configuration.Metadata
{
	public class BasicMetadataGenerator 
	{
		public bool AddBasic(XElement parent, PropertyAuditingData propertyAuditingData,
					 IValue value, ISimpleMapperBuilder mapper, bool insertable, bool key) 
		{
			var type = value.Type;
			var custType = type as CustomType;
			var compType = type as CompositeCustomType;
			if (type is ImmutableType || type is MutableType)
			{
				var mappingType = type.GetType();
				var userDefined = isUserDefined(mappingType);
				if (userDefined)
				{
					addCustomValue(parent, propertyAuditingData, value, mapper, insertable, key, mappingType);
				}
				else
				{
					addSimpleValue(parent, propertyAuditingData, value, mapper, insertable, key);					
				}
			}
			else if (custType != null)
			{
				addCustomValue(parent, propertyAuditingData, value, mapper, insertable, key, custType.UserType.GetType());
			}
			else if (compType != null)
			{
				addCustomValue(parent, propertyAuditingData, value, mapper, insertable, key, compType.UserType.GetType());
			}
			else 
			{
				return false;
			}

			return true;
		}

		private static bool isUserDefined(System.Type mappingType)
		{
			if (isUserDefinedInner(mappingType))
			{
				return true;
			}
			return mappingType.IsGenericType && mappingType.GetGenericArguments().Any(isUserDefinedInner);
		}

		private static bool isUserDefinedInner(System.Type mappingType)
		{
			return !typeof(ISession).Assembly.Equals(mappingType.Assembly);
		}
	

		private static void addSimpleValue(XElement parent, PropertyAuditingData propertyAuditingData,
								IValue value, ISimpleMapperBuilder mapper, bool insertable, bool key)
		{
			if (parent != null) 
			{
				var propMapping = MetadataTools.AddProperty(parent, propertyAuditingData.Name,
						value.Type.Name, propertyAuditingData.ForceInsertable || insertable, key);
				MetadataTools.AddColumns(propMapping, value.ColumnIterator.OfType<Column>());
			}

			// A null mapper means that we only want to add xml mappings
			if (mapper != null) 
			{
				mapper.Add(propertyAuditingData.GetPropertyData());
			}
		}

		private static void addCustomValue(XElement parent, PropertyAuditingData propertyAuditingData,
									IValue value, ISimpleMapperBuilder mapper, bool insertable, 
									bool key, System.Type typeOfUserImplementation) 
		{
			if (parent != null) 
			{
				var propMapping = MetadataTools.AddProperty(parent, propertyAuditingData.Name,
						typeOfUserImplementation.AssemblyQualifiedName, insertable, key);
				MetadataTools.AddColumns(propMapping, value.ColumnIterator.OfType<Column>());
				var typeElement = new XElement(MetadataTools.CreateElementName("type"), 
											new XAttribute("name", typeOfUserImplementation.AssemblyQualifiedName));

				var simpleValue = value as SimpleValue;
				if (simpleValue != null) 
				{
					var typeParameters = simpleValue.TypeParameters;
					if (typeParameters != null) 
					{
						foreach (var paramKeyValue in typeParameters) 
						{
							var typeParam = new XElement(MetadataTools.CreateElementName("param"),
								new XAttribute("name", paramKeyValue.Key), paramKeyValue.Value);
							typeElement.Add(typeParam);
						}
					}
				}
				propMapping.Add(typeElement);
			}

			if (mapper != null) 
			{
				mapper.Add(propertyAuditingData.GetPropertyData());
			}
		}

		public void AddKeyManyToOne(XElement parent, PropertyAuditingData propertyAuditingData, IValue value, ISimpleMapperBuilder mapper)
		{
			var type = value.Type;
			var element = mapper == null ? 
				  MetadataTools.AddKeyManyToOne(parent, propertyAuditingData.Name, type.ReturnedClass.AssemblyQualifiedName) : 
				  MetadataTools.AddManyToOne(parent, propertyAuditingData.Name, type.ReturnedClass.AssemblyQualifiedName, true, false);
			MetadataTools.AddColumns(element, value.ColumnIterator.OfType<Column>());
			// A null mapper occurs when adding to composite-id element
			if (mapper != null)
			{
				mapper.Add(propertyAuditingData.GetPropertyData());
			}
		}
	}
}
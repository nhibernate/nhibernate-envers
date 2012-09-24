using System.Linq;
using System.Xml;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Mapping;
using NHibernate.Type;

namespace NHibernate.Envers.Configuration.Metadata
{
	public class BasicMetadataGenerator 
	{
		public bool AddBasic(XmlElement parent, PropertyAuditingData propertyAuditingData,
					 IValue value, ISimpleMapperBuilder mapper, bool insertable, bool key) 
		{
			var type = value.Type;
			var custType = type as CustomType;
			var compType = type as CompositeCustomType;
			if (type is ImmutableType || type is MutableType)
			{
				var mappingType = type.GetType();
				var userDefined = isUserDefined(mappingType);
				if(userDefined)
					addCustomValue(parent, propertyAuditingData, value, mapper, insertable, key, mappingType);
				else
					addSimpleValue(parent, propertyAuditingData, value, mapper, insertable, key);
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
	

		private static void addSimpleValue(XmlElement parent, PropertyAuditingData propertyAuditingData,
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

		private static void addCustomValue(XmlElement parent, PropertyAuditingData propertyAuditingData,
									IValue value, ISimpleMapperBuilder mapper, bool insertable, 
									bool key, System.Type typeOfUserImplementation) 
		{
			if (parent != null) 
			{
				var propMapping = MetadataTools.AddProperty(parent, propertyAuditingData.Name,
						typeOfUserImplementation.AssemblyQualifiedName, insertable, key);
				MetadataTools.AddColumns(propMapping, value.ColumnIterator.OfType<Column>());
				var typeElement = parent.OwnerDocument.CreateElement("type");
				typeElement.SetAttribute("name", typeOfUserImplementation.AssemblyQualifiedName);

				var simpleValue = value as SimpleValue;
				if (simpleValue != null) 
				{
					var typeParameters = simpleValue.TypeParameters;
					if (typeParameters != null) 
					{
						foreach (var paramKeyValue in typeParameters) 
						{
							var typeParam = typeElement.OwnerDocument.CreateElement("param");
							typeParam.SetAttribute("name", paramKeyValue.Key);
							typeParam.InnerText =  paramKeyValue.Value;
							typeElement.AppendChild(typeParam);
						}
					}
				}
				propMapping.AppendChild(typeElement);
			}

			if (mapper != null) 
			{
				mapper.Add(propertyAuditingData.GetPropertyData());
			}
		}
	}
}
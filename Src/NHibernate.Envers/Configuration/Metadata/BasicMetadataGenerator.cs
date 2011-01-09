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
				AddSimpleValue(parent, propertyAuditingData, value, mapper, insertable, key);
			}
			else if (custType != null)
			{
				AddCustomValue(parent, propertyAuditingData, value, mapper, insertable, key, custType.UserType.GetType());
			}
			else if (compType != null)
			{
				AddCustomValue(parent, propertyAuditingData, value, mapper, insertable, key, compType.UserType.GetType());
			}
			// TODO Simon: There is no equivalent of PrimitiveByteArrayBlobType in NHibernate, will see later if needed
			// ORIG:
			//else if ("org.hibernate.type.PrimitiveByteArrayBlobType".equals(type.getClass().getName()))
			//{
			//    AddSimpleValue(parent, propertyAuditingData, value, mapper, insertable, key);
			//}
			else 
			{
				return false;
			}

			return true;
		}

		private void AddSimpleValue(XmlElement parent, PropertyAuditingData propertyAuditingData,
								IValue value, ISimpleMapperBuilder mapper, bool insertable, bool key)
		{
			if (parent != null) 
			{
				var prop_mapping = MetadataTools.AddProperty(parent, propertyAuditingData.Name,
						value.Type.Name, propertyAuditingData.ForceInsertable || insertable, key);
				MetadataTools.AddColumns(prop_mapping, value.ColumnIterator.OfType<Column>());
			}

			// A null mapper means that we only want to add xml mappings
			if (mapper != null) 
			{
				mapper.Add(propertyAuditingData.getPropertyData());
			}
		}

		private void AddCustomValue(XmlElement parent, PropertyAuditingData propertyAuditingData,
									IValue value, ISimpleMapperBuilder mapper, bool insertable, 
									bool key, System.Type typeOfUserImplementation) 
		{
			if (parent != null) 
			{
				var prop_mapping = MetadataTools.AddProperty(parent, propertyAuditingData.Name,
						null, insertable, key);
				MetadataTools.AddColumns(prop_mapping, value.ColumnIterator.OfType<Column>());
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
							var type_param = typeElement.OwnerDocument.CreateElement("param");
							type_param.SetAttribute("name", paramKeyValue.Key);
							type_param.InnerText =  paramKeyValue.Value;
							typeElement.AppendChild(type_param);
						}
					}
				}
				prop_mapping.AppendChild(typeElement);
			}

			if (mapper != null) 
			{
				mapper.Add(propertyAuditingData.getPropertyData());
			}
		}
	}
}

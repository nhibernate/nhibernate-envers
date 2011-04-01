﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata
{
	public static class MetadataTools 
	{
		public static XmlElement AddNativelyGeneratedId(XmlDocument doc, XmlElement parent, string name, string type) 
		{
			var id_mapping = doc.CreateElement("id");
			parent.AppendChild(id_mapping);
			id_mapping.SetAttribute("name", name);
			id_mapping.SetAttribute("type", type);

			var generator_mapping = doc.CreateElement("generator");
			id_mapping.AppendChild(generator_mapping);
			generator_mapping.SetAttribute("class", "native");
			/*generator_mapping.SetAttribute("class", "sequence");
			generator_mapping.addElement("param").SetAttribute("name", "sequence").setText("custom");*/

			return id_mapping;
		}

		public static XmlElement AddProperty(XmlElement parent, string name, string type, bool insertable, bool updateable, bool key)
		{
			XmlElement prop_mapping;
			if (key)
			{
				prop_mapping = parent.OwnerDocument.CreateElement("key-property");
			}
			else
			{
				prop_mapping = parent.OwnerDocument.CreateElement("property");
				prop_mapping.SetAttribute("insert", insertable ? "true" : "false");
				prop_mapping.SetAttribute("update", updateable ? "true" : "false");
			}
			parent.AppendChild(prop_mapping);

			prop_mapping.SetAttribute("name", name);

			if (type != null)
			{
				prop_mapping.SetAttribute("type", type);
			}

			return prop_mapping;
		}

		public static XmlElement AddProperty(XmlElement parent, string name, string type, bool insertable, bool key)
		{
			return AddProperty(parent, name, type, insertable, false, key);
		}

		public static XmlElement AddManyToOne(XmlElement parent, string name, string type, bool insertable, bool updateable)
		{
			var manyToOneMapping = parent.OwnerDocument.CreateElement("many-to-one");
			parent.AppendChild(manyToOneMapping);
			manyToOneMapping.SetAttribute("insert", insertable ? "true" : "false");
			manyToOneMapping.SetAttribute("update", updateable ? "true" : "false");
			manyToOneMapping.SetAttribute("name", name);
			manyToOneMapping.SetAttribute("class", type);
			return manyToOneMapping;
		}

		private static void AddOrModifyAttribute(XmlElement parent, string name, string value) 
		{
			parent.SetAttribute(name,value);
		}

		public static XmlElement AddOrModifyColumn(XmlElement parent, string name) 
		{
			var column_mapping = (XmlElement)parent.SelectSingleNode("column");

			if (column_mapping == null) 
			{
				return AddColumn(parent, name, -1, 0, 0, null);
			}

			if (!string.IsNullOrEmpty(name)) 
			{
				AddOrModifyAttribute(column_mapping, "name", name);
			}

			return column_mapping;
		}

		public static XmlElement AddColumn(XmlElement parent, string name, int length, int scale, int precision, string sqlType) 
		{
			var column_mapping = parent.OwnerDocument.CreateElement("column");
			parent.AppendChild(column_mapping);

			column_mapping.SetAttribute("name", name);
			if (length != -1) 
			{
				column_mapping.SetAttribute("length", length.ToString());
			}
			if (scale != 0) 
			{
				column_mapping.SetAttribute("scale", scale.ToString());
			}
			if (precision != 0) 
			{
				column_mapping.SetAttribute("precision", precision.ToString());
			}
			if (!string.IsNullOrEmpty(sqlType)) 
			{
				column_mapping.SetAttribute("sql-type", sqlType);
			}

			return column_mapping;
		}

		private static XmlElement CreateEntityCommon(XmlDocument document, string type, AuditTableData auditTableData, string discriminatorValue) 
		{
			var hibernate_mapping = document.CreateElement("hibernate-mapping");
			hibernate_mapping.SetAttribute("assembly", "NHibernate.Envers");
			hibernate_mapping.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
			hibernate_mapping.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			hibernate_mapping.SetAttribute("xmlns", "urn:nhibernate-mapping-2.2"); 
			hibernate_mapping.SetAttribute("auto-import", "false");
			document.AppendChild(hibernate_mapping);

			var class_mapping = document.CreateElement(type);
			hibernate_mapping.AppendChild(class_mapping);

			if (auditTableData.AuditEntityName != null) 
			{
				class_mapping.SetAttribute("entity-name", auditTableData.AuditEntityName);
			}

			if (discriminatorValue != null) 
			{
				class_mapping.SetAttribute("discriminator-value", discriminatorValue);
			}

			if (!string.IsNullOrEmpty(auditTableData.AuditTableName)) 
			{
				class_mapping.SetAttribute("table", auditTableData.AuditTableName);
			}

			if (!string.IsNullOrEmpty(auditTableData.Schema)) 
			{
				class_mapping.SetAttribute("schema", auditTableData.Schema);
			}

			if (!string.IsNullOrEmpty(auditTableData.Catalog)) 
			{
				class_mapping.SetAttribute("catalog", auditTableData.Catalog);
			}

			return class_mapping;
		}

		public static XmlElement CreateEntity(XmlDocument document, AuditTableData auditTableData, string discriminatorValue) 
		{
			return CreateEntityCommon(document, "class", auditTableData, discriminatorValue);
		}

		public static XmlElement CreateSubclassEntity(XmlDocument document, string subclassType, AuditTableData auditTableData,
												   string extendsEntityName, string discriminatorValue) 
		{
			var class_mapping = CreateEntityCommon(document, subclassType, auditTableData, discriminatorValue);
			class_mapping.SetAttribute("extends", extendsEntityName);

			return class_mapping;
		}

		public static XmlElement CreateJoin(XmlElement parent, string tableName,
										 string schema, string catalog) 
		{
			var join_mapping = parent.OwnerDocument.CreateElement("join");
			parent.AppendChild(join_mapping);

			join_mapping.SetAttribute("table", tableName);

			if (!string.IsNullOrEmpty(schema)) 
			{
				join_mapping.SetAttribute("schema", schema);
			}

			if (!string.IsNullOrEmpty(catalog)) 
			{
				join_mapping.SetAttribute("catalog", catalog);
			}

			return join_mapping;
		}

		/// <summary>
		/// Adds the columns in the enumerator to the any_mapping XmlElement
		/// </summary>
		/// <param name="any_mapping"></param>
		/// <param name="columns">should contain elements of Column type</param>
		public static void AddColumns(XmlElement any_mapping, IEnumerable<Column> columns)
		{
			foreach (var column in columns)
			{
				AddColumn(any_mapping, column.Name, column.Length, column.IsPrecisionDefined() ? column.Scale : 0,
						column.IsPrecisionDefined() ? column.Precision : 0, column.SqlType);
			}
		}

		private static void ChangeNamesInColumnElement(XmlElement element, IEnumerator<string> columnEnumerator) 
		{
			var nodeList = element.ChildNodes;
			foreach (XmlElement property in nodeList)
			{
				if ("column".Equals(property.Name)) 
				{
					var value = property.GetAttribute("name");
					if (!string.IsNullOrEmpty(value))
					{
						columnEnumerator.MoveNext();
						property.SetAttribute("name", columnEnumerator.Current);
					}
				}
			}
		}

		public static void PrefixNamesInPropertyElement(XmlElement element, string prefix, IEnumerator<string> columnNames,
														bool changeToKey, bool insertable) 
		{
			var nodeList = element.ChildNodes;

			for (var i = nodeList.Count - 1; i >= 0; i--)
			{
				var property = (XmlElement) nodeList[i];
				if ("property".Equals(property.Name))
				{
					var value = property.GetAttribute("name");
					if (!string.IsNullOrEmpty(value))
					{
						property.SetAttribute("name", prefix + value);
					}
					ChangeNamesInColumnElement(property, columnNames);

					if (changeToKey)
					{
						ChangeToKeyProperty(property);
					}
					else
					{
						property.SetAttribute("insert", insertable ? "true" : "false");
					}
				}
			}
		}

		private static void ChangeToKeyProperty(XmlElement element)
		{
			var newElement = element.OwnerDocument.CreateElement("key-property");
			foreach (XmlAttribute attribute in element.Attributes)
			{
				var attrName = attribute.Name;
				if (!attrName.Equals("insert") && !attrName.Equals("update"))
					newElement.SetAttributeNode((XmlAttribute)attribute.CloneNode(true));
			}
			for (var i = 0; i < element.ChildNodes.Count; i++) 
			{
				newElement.AppendChild(element.ChildNodes[i].CloneNode(true));
			}
			var parent = element.ParentNode;
			parent.RemoveChild(element);
			parent.AppendChild(newElement);
		}

		public static IEnumerator<string> GetColumnNameEnumerator(IEnumerable<ISelectable> columns)
		{
			return (from Column column in columns select column.Name).GetEnumerator();
		}
	}
}

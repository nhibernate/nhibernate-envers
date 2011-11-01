using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata
{
	public static class MetadataTools 
	{
		public static XmlElement AddNativelyGeneratedId(XmlDocument doc, XmlElement parent, string name, System.Type type) 
		{
			var idMapping = doc.CreateElement("id");
			parent.AppendChild(idMapping);
			idMapping.SetAttribute("name", name);
			idMapping.SetAttribute("type", type.Name);

			var generatorMapping = doc.CreateElement("generator");
			idMapping.AppendChild(generatorMapping);
			generatorMapping.SetAttribute("class", "native");

			return idMapping;
		}

		public static XmlElement AddProperty(XmlElement parent, string name, string type, bool insertable, bool updateable, bool key)
		{
			XmlElement propMapping;
			if (key)
			{
				propMapping = parent.OwnerDocument.CreateElement("key-property");
			}
			else
			{
				propMapping = parent.OwnerDocument.CreateElement("property");
				propMapping.SetAttribute("insert", insertable ? "true" : "false");
				propMapping.SetAttribute("update", updateable ? "true" : "false");
			}
			parent.AppendChild(propMapping);

			propMapping.SetAttribute("name", name);

			if (type != null)
			{
				propMapping.SetAttribute("type", type);
			}

			return propMapping;
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

		public static void AddFormula(XmlElement element, Formula formula)
		{
			element.SetAttribute("formula", formula.Text);
		}

		public static XmlElement AddColumn(XmlElement parent, string name, int length, int scale, int precision, string sqlType) 
		{
			var columnMapping = parent.OwnerDocument.CreateElement("column");
			parent.AppendChild(columnMapping);

			columnMapping.SetAttribute("name", name);
			if (length != -1) 
			{
				columnMapping.SetAttribute("length", length.ToString());
			}
			if (scale != 0) 
			{
				columnMapping.SetAttribute("scale", scale.ToString());
			}
			if (precision != 0) 
			{
				columnMapping.SetAttribute("precision", precision.ToString());
			}
			if (!string.IsNullOrEmpty(sqlType)) 
			{
				columnMapping.SetAttribute("sql-type", sqlType);
			}

			return columnMapping;
		}

		private static XmlElement CreateEntityCommon(XmlDocument document, string type, AuditTableData auditTableData, string discriminatorValue) 
		{
			var hibernateMapping = document.CreateElement("hibernate-mapping");
			hibernateMapping.SetAttribute("assembly", "NHibernate.Envers");
			hibernateMapping.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
			hibernateMapping.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			hibernateMapping.SetAttribute("xmlns", "urn:nhibernate-mapping-2.2"); 
			hibernateMapping.SetAttribute("auto-import", "false");
			document.AppendChild(hibernateMapping);

			var classMapping = document.CreateElement(type);
			hibernateMapping.AppendChild(classMapping);

			if (auditTableData.AuditEntityName != null) 
			{
				classMapping.SetAttribute("entity-name", auditTableData.AuditEntityName);
			}

			if (discriminatorValue != null) 
			{
				classMapping.SetAttribute("discriminator-value", discriminatorValue);
			}

			if (!string.IsNullOrEmpty(auditTableData.AuditTableName)) 
			{
				classMapping.SetAttribute("table", auditTableData.AuditTableName);
			}

			if (!string.IsNullOrEmpty(auditTableData.Schema)) 
			{
				classMapping.SetAttribute("schema", auditTableData.Schema);
			}

			if (!string.IsNullOrEmpty(auditTableData.Catalog)) 
			{
				classMapping.SetAttribute("catalog", auditTableData.Catalog);
			}

			return classMapping;
		}

		public static XmlElement CreateEntity(XmlDocument document, AuditTableData auditTableData, string discriminatorValue) 
		{
			return CreateEntityCommon(document, "class", auditTableData, discriminatorValue);
		}

		public static XmlElement CreateSubclassEntity(XmlDocument document, string subclassType, AuditTableData auditTableData,
												   string extendsEntityName, string discriminatorValue) 
		{
			var classMapping = CreateEntityCommon(document, subclassType, auditTableData, discriminatorValue);
			classMapping.SetAttribute("extends", extendsEntityName);

			return classMapping;
		}

		public static XmlElement CreateJoin(XmlElement parent, string tableName,
										 string schema, string catalog) 
		{
			var joinMapping = parent.OwnerDocument.CreateElement("join");
			parent.AppendChild(joinMapping);

			joinMapping.SetAttribute("table", tableName);

			if (!string.IsNullOrEmpty(schema)) 
			{
				joinMapping.SetAttribute("schema", schema);
			}

			if (!string.IsNullOrEmpty(catalog)) 
			{
				joinMapping.SetAttribute("catalog", catalog);
			}

			return joinMapping;
		}

		/// <summary>
		/// Adds the columns in the enumerator to the any_mapping XmlElement
		/// </summary>
		/// <param name="anyMapping"></param>
		/// <param name="columns">should contain elements of Column type</param>
		public static void AddColumns(XmlElement anyMapping, IEnumerable<Column> columns)
		{
			foreach (var column in columns)
			{
				AddColumn(anyMapping, column);
			}
		}

		public static void AddColumn(XmlElement anyMapping, Column column)
		{
			AddColumn(anyMapping, column.Name, column.Length, column.IsPrecisionDefined() ? column.Scale : 0,
					column.IsPrecisionDefined() ? column.Precision : 0, column.SqlType);
		}

		public static void AddColumnsOrFormulas(XmlElement element, IEnumerable<ISelectable> columnIterator)
		{
			foreach (var selectable in columnIterator)
			{
				var column = selectable as Column;
				if (column != null)
					AddColumn(element, column);
				else
					AddFormula(element, (Formula) selectable);
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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata
{
	public static class MetadataTools
	{
		private static readonly XNamespace xNamespace = "urn:nhibernate-mapping-2.2";

		public static XName CreateElementName(string name)
		{
			return xNamespace + name;
		}

		public static void AddNativelyGeneratedId(XElement parent, string name, System.Type type)
		{
			var idMapping = new XElement(CreateElementName("id"),
												new XAttribute("name", name),
												new XAttribute("column", "REV"),
												new XAttribute("type", type.Name),
													new XElement(CreateElementName("generator"), 
														new XAttribute("class", "native"),
															new XElement(CreateElementName("param"),
																new XAttribute("name", "sequence"),
																	"rev_id_seq")));
			parent.Add(idMapping);
		}

		public static XElement AddProperty(XElement parent, string name, string type, bool insertable, bool updateable, bool key)
		{
			XElement propMapping;
			if (key)
			{
				propMapping = new XElement(CreateElementName("key-property"));
			}
			else
			{
				propMapping = new XElement(CreateElementName("property"), 
									new XAttribute("insert", insertable ? "true" : "false"),	
									new XAttribute("update", updateable ? "true" : "false"));
			}
			parent.Add(propMapping);

			propMapping.Add(new XAttribute("name", name));

			if (type != null)
			{
				propMapping.Add(new XAttribute("type", type));
			}

			return propMapping;
		}

		public static XElement AddProperty(XElement parent, string name, string type, bool insertable, bool key)
		{
			return AddProperty(parent, name, type, insertable, false, key);
		}

		public static XElement AddManyToOne(XElement parent, string name, string type, bool insertable, bool updateable)
		{
			var manyToOneMapping = new XElement(CreateElementName("many-to-one"),
				new XAttribute("insert", insertable ? "true" : "false"),
				new XAttribute("update", updateable ? "true" : "false"),
				new XAttribute("name", name),
				new XAttribute("class", type));
			parent.Add(manyToOneMapping);
			return manyToOneMapping;
		}

		public static XElement AddKeyManyToOne(XElement parent, string name, string type)
		{
			var manyToOneMapping = new XElement(CreateElementName("key-many-to-one"),
				new XAttribute("name", name),
				new XAttribute("class", type));
			parent.Add(manyToOneMapping);
			return manyToOneMapping;
		}

		private static void addOrModifyAttribute(XElement parent, string name, string value)
		{
			parent.SetAttributeValue(name, value);
		}

		/// <summary>
		/// Column name shall be wrapped with '`' signs if quotation required.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static void AddOrModifyColumn(XElement parent, string name)
		{
			var columnMapping = parent.Element(CreateElementName("column"));

			if (columnMapping == null)
			{
				AddColumn(parent, name, -1, -1, -1, null, false);
			}
			else if (!string.IsNullOrEmpty(name))
			{
				addOrModifyAttribute(columnMapping, "name", name);
			}
		}

		private static void addFormula(XElement element, Formula formula)
		{
			element.Add(new XAttribute("formula", formula.Text));
		}

		public static void AddColumn(XElement parent, string name, int length, int scale, int precision, string sqlType, bool quoted)
		{
			var columnMapping = new XElement(CreateElementName("column"),
														new XAttribute("name", quoted ? "`" + name + "`" : name));
			parent.Add(columnMapping);
			if (length != -1)
			{
				columnMapping.Add(new XAttribute("length", length.ToString()));
			}
			if (scale != -1)
			{
				columnMapping.Add(new XAttribute("scale", scale.ToString()));
			}
			if (precision != -1)
			{
				columnMapping.Add(new XAttribute("precision", precision.ToString()));
			}
			if (!string.IsNullOrEmpty(sqlType))
			{
				columnMapping.Add(new XAttribute("sql-type", sqlType));
			}
		}

		private static XElement createEntityCommon(XDocument document, 
																	string type, 
																	AuditTableData auditTableData, 
																	string discriminatorValue,
																	bool isAbstract)
		{
			var classMapping = new XElement(CreateElementName(type));
			var nhMapping = new XElement(CreateElementName("hibernate-mapping"),
															new XAttribute("assembly", "NHibernate.Envers"),
			                        new XAttribute("auto-import", "false"),
																	 classMapping);
			document.Add(nhMapping);

			if (isAbstract)
			{
				classMapping.Add(new XAttribute("abstract", "true"));
			}

			if (auditTableData.AuditEntityName != null)
			{
				classMapping.Add(new XAttribute("entity-name", auditTableData.AuditEntityName));
			}

			if (discriminatorValue != null)
			{
				classMapping.Add(new XAttribute("discriminator-value", discriminatorValue));
			}

			if (!string.IsNullOrEmpty(auditTableData.AuditTableName))
			{
				classMapping.Add(new XAttribute("table", auditTableData.AuditTableName));
			}

			if (!string.IsNullOrEmpty(auditTableData.Schema) && !type.Equals("subclass"))
			{
				classMapping.Add(new XAttribute("schema", auditTableData.Schema));
			}

			if (!string.IsNullOrEmpty(auditTableData.Catalog) && !type.Equals("subclass"))
			{
				classMapping.Add(new XAttribute("catalog", auditTableData.Catalog));
			}

			return classMapping;
		}

		public static XElement CreateEntity(XDocument document, AuditTableData auditTableData, string discriminatorValue, bool isAbstract)
		{
			return createEntityCommon(document, "class", auditTableData, discriminatorValue, isAbstract);
		}

		public static XElement CreateSubclassEntity(XDocument document, string subclassType, AuditTableData auditTableData,
													string extendsEntityName, string discriminatorValue, bool isAbstract)
		{
			var classMapping = createEntityCommon(document, subclassType, auditTableData, discriminatorValue, isAbstract);
			classMapping.Add(new XAttribute("extends", extendsEntityName));

			return classMapping;
		}

		public static XElement CreateJoin(XElement parent, string tableName,
										 string schema, string catalog)
		{
			var joinMapping = new XElement(CreateElementName("join"), new XAttribute("table", tableName));
			parent.Add(joinMapping);

			if (!string.IsNullOrEmpty(schema))
			{
				joinMapping.Add(new XAttribute("schema", schema));
			}

			if (!string.IsNullOrEmpty(catalog))
			{
				joinMapping.Add(new XAttribute("catalog", catalog));
			}

			return joinMapping;
		}

		/// <summary>
		/// Adds the columns in the enumerator to the any_mapping XmlElement
		/// </summary>
		/// <param name="anyMapping"></param>
		/// <param name="columns">should contain elements of Column type</param>
		public static void AddColumns(XElement anyMapping, IEnumerable<Column> columns)
		{
			foreach (var column in columns)
			{
				addColumn(anyMapping, column);
			}
		}

		private static void addColumn(XElement anyMapping, Column column)
		{
			AddColumn(anyMapping, column.Name, column.IsLengthDefined() ? column.Length : -1, column.IsPrecisionDefined() ? column.Scale : -1,
						column.IsPrecisionDefined() ? column.Precision : -1, column.SqlType, column.IsQuoted);
		}

		public static void AddColumnsOrFormulas(XElement element, IEnumerable<ISelectable> columnIterator)
		{
			foreach (var selectable in columnIterator)
			{
				var column = selectable as Column;
				if (column != null)
					addColumn(element, column);
				else
					addFormula(element, (Formula)selectable);
			}
		}

		private static void changeNamesInColumnElement(XElement element, IEnumerator<string> columnEnumerator)
		{
			foreach (var property in element.Elements())
			{
				if ("column".Equals(property.Name.LocalName))
				{
					var value = property.Attribute("name");
					if (!string.IsNullOrEmpty(value.Value))
					{
						columnEnumerator.MoveNext();
						property.SetAttributeValue("name", columnEnumerator.Current);
					}
				}
			}
		}

		public static void PrefixNamesInPropertyElement(XElement element, string prefix, IEnumerable<string> columnNames,
														bool changeToKey, bool insertable)
		{
			var nodeList = new List<XElement>(element.Elements());
			//need to reverse names because looping backwards
			using (var columnNamesReversed = columnNames.Reverse().GetEnumerator())
			{
				for (var i = nodeList.Count - 1; i >= 0; i--)
				{
					var property = nodeList[i];
					var propertyName = property.Name;
					if ("property".Equals(propertyName.LocalName) || "many-to-one".Equals(propertyName.LocalName))
					{
						var value = property.Attribute("name").Value;
						if (!string.IsNullOrEmpty(value))
						{
							property.SetAttributeValue("name", prefix + value);
						}
						changeNamesInColumnElement(property, columnNamesReversed);

						if (changeToKey)
						{
							changeToKeyProperty(property);
						}
						else
						{
							property.SetAttributeValue("insert", insertable ? "true" : "false");
						}
					}
				}
			}
		}

		private static void changeToKeyProperty(XElement element)
		{
			var newElement = new XElement(CreateElementName("key-property"), element.Elements());
			foreach (var attribute in element.Attributes())
			{
				var attrName = attribute.Name;
				if (!attrName.LocalName.Equals("insert") && !attrName.LocalName.Equals("update"))
					newElement.Add(attribute);
			}
			element.Parent.Add(newElement);
			element.Remove();
		}

		public static IEnumerable<string> GetColumnNameEnumerator(IEnumerable<ISelectable> columns)
		{
			return (from Column column in columns select column.Name);
		}

		public static void AddModifiedFlagProperty(XElement parent, string propertyName, string suffix)
		{
			AddProperty(parent, ModifiedFlagPropertyName(propertyName, suffix), "bool", true, false, false);
		}

		public static string ModifiedFlagPropertyName(string propertyName, string suffix)
		{
			return propertyName + suffix;
		}
	}
}

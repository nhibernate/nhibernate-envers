using System.Collections.Generic;
using System.Xml.Linq;

namespace NHibernate.Envers.Configuration.Metadata
{
	public class EntityXmlMappingData
	{
		public XDocument MainXmlMapping { get; private set; }
		public IList<XDocument> AdditionalXmlMappings { get; private set; }

		/// <summary>
		/// The xml element that maps the class. The root can be one of the folowing elements:
		/// class, subclass, union-subclass, joined-subclass
		/// </summary>
		public XElement ClassMapping { get; set; }

		public EntityXmlMappingData()
		{
			MainXmlMapping = new XDocument();
			AdditionalXmlMappings = new List<XDocument>();
		}

		public XDocument NewAdditionalMapping()
		{
			var additionalMapping = new XDocument();
			AdditionalXmlMappings.Add(additionalMapping);

			return additionalMapping;
		}
	}
}

using System.Collections.Generic;
using System.Xml;

namespace NHibernate.Envers.Configuration.Metadata
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class EntityXmlMappingData
    {
        public XmlDocument MainXmlMapping { get; private set; }
        public IList<XmlDocument> AdditionalXmlMappings { get; private set; }
        /**
         * The xml element that maps the class. The root can be one of the folowing elements:
         * class, subclass, union-subclass, joined-subclass
         */
        public XmlElement ClassMapping { get; set; }

        public EntityXmlMappingData()
        {
            MainXmlMapping = new XmlDocument();
            AdditionalXmlMappings = new List<XmlDocument>();
        }

        public XmlDocument NewAdditionalMapping()
        {
            var additionalMapping = new XmlDocument();
            AdditionalXmlMappings.Add(additionalMapping);

            return additionalMapping;
        }
    }
}

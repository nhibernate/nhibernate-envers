using System.Xml;
using NHibernate.Envers.Entities.Mapper.Id;

namespace NHibernate.Envers.Entities
{
	public class IdMappingData 
	{
		public IdMappingData(IIdMapper idMapper, XmlElement xmlMapping, XmlElement xmlRelationMapping)
		{
			IdMapper = idMapper;
			XmlMapping = xmlMapping;
			XmlRelationMapping = xmlRelationMapping;
		}

		public IIdMapper IdMapper { get; private set; }
		// Mapping which will be used to generate the entity
		public XmlElement XmlMapping { get; private set; }
		// Mapping which will be used to generate references to the entity in related entities
		public XmlElement XmlRelationMapping { get; private set; }
	}
}

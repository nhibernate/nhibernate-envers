using System;
using System.Xml.Linq;
using NHibernate.Envers.Entities.Mapper.Id;

namespace NHibernate.Envers.Entities
{
	[Serializable]
	public class IdMappingData 
	{
		public IdMappingData(IIdMapper idMapper, XElement xmlMapping, XElement xmlRelationMapping)
		{
			IdMapper = idMapper;
			XmlMapping = xmlMapping;
			XmlRelationMapping = xmlRelationMapping;
		}

		public IIdMapper IdMapper { get; private set; }
		// Mapping which will be used to generate the entity
		[NonSerialized]
		private XElement _xmlMapping;
		public XElement XmlMapping
		{
			get { return _xmlMapping; }
			private set { _xmlMapping = value; }
		}

		// Mapping which will be used to generate references to the entity in related entities
		[NonSerialized]
		private XElement _xmlRelationMapping;
		public XElement XmlRelationMapping
		{
			get { return _xmlRelationMapping; }
			private set { _xmlRelationMapping = value; }
		}
	}
}

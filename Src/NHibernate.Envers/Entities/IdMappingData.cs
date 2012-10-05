using System;
using System.Xml;
using NHibernate.Envers.Entities.Mapper.Id;

namespace NHibernate.Envers.Entities
{
	[Serializable]
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
		[NonSerialized]
		private XmlElement _xmlMapping;
		public XmlElement XmlMapping
		{
			get { return _xmlMapping; }
			private set { _xmlMapping = value; }
		}

		// Mapping which will be used to generate references to the entity in related entities
		[NonSerialized]
		private XmlElement _xmlRelationMapping;
		public XmlElement XmlRelationMapping
		{
			get { return _xmlRelationMapping; }
			private set { _xmlRelationMapping = value; }
		}
	}
}

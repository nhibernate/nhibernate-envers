using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Entities.Mapper;

namespace NHibernate.Envers.Entities
{
	public class RelationDescription 
	{
		public RelationDescription(string fromPropertyName, RelationType relationType, string toEntityName,
						   string mappedByPropertyName, IIdMapper idMapper,
						   IPropertyMapper fakeBidirectionalRelationMapper,
						   IPropertyMapper fakeBidirectionalRelationIndexMapper, bool insertable)
		{
			FromPropertyName = fromPropertyName;
			RelationType = relationType;
			ToEntityName = toEntityName;
			MappedByPropertyName = mappedByPropertyName;
			IdMapper = idMapper;
			FakeBidirectionalRelationMapper = fakeBidirectionalRelationMapper;
			FakeBidirectionalRelationIndexMapper = fakeBidirectionalRelationIndexMapper;
			Insertable = insertable;

			Bidirectional = false;
		}

		public string FromPropertyName { get; private set;}
		public RelationType RelationType { get; private set; }
		public string ToEntityName { get; private set; }
		public string MappedByPropertyName { get; private set; }
		public IIdMapper IdMapper { get; private set; }
		public IPropertyMapper FakeBidirectionalRelationMapper { get; private set; }
		public IPropertyMapper FakeBidirectionalRelationIndexMapper { get; private set; }
		public bool Insertable { get; private set; }
		public bool Bidirectional { get; set; }
	}
}

using System;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Id;

namespace NHibernate.Envers.Entities
{
	[Serializable]
	public class RelationDescription 
	{
		public static RelationDescription ToOne(string fromPropertyName, RelationType relationType, string toEntityName,
		                    string mappedByPropertyName, IIdMapper idMapper,
		                    IPropertyMapper fakeBidirectionalRelationMapper,
		                    IPropertyMapper fakeBidirectionalRelationIndexMapper, bool insertable, bool ignoreNotFound)
		{
			return new RelationDescription(fromPropertyName, relationType, toEntityName, mappedByPropertyName, idMapper,
			                               fakeBidirectionalRelationMapper, fakeBidirectionalRelationIndexMapper, insertable,
			                               ignoreNotFound);
		}

		public static RelationDescription ToMany(string fromPropertyName, RelationType relationType, string toEntityName,
		                                         string mappedByPropertyName, IIdMapper idMapper,
		                                         IPropertyMapper fakeBidirectionalRelationMapper,
		                                         IPropertyMapper fakeBidirectionalRelationIndexMapper, bool insertable)
		{
			// Envers populates collections by executing dedicated queries. Special handling of
			// @NotFound(action = NotFoundAction.IGNORE) can be omitted in such case as exceptions
			// (e.g. EntityNotFoundException, ObjectNotFoundException) are never thrown.
			// Therefore assigning false to ignoreNotFound.
			return new RelationDescription(fromPropertyName, relationType, toEntityName, mappedByPropertyName, idMapper,
															 fakeBidirectionalRelationMapper, fakeBidirectionalRelationIndexMapper, insertable,
															 false);
		}

		private RelationDescription(string fromPropertyName, RelationType relationType, string toEntityName,
						   string mappedByPropertyName, IIdMapper idMapper,
						   IPropertyMapper fakeBidirectionalRelationMapper,
						   IPropertyMapper fakeBidirectionalRelationIndexMapper, bool insertable, bool ignoreNotFound)
		{
			FromPropertyName = fromPropertyName;
			RelationType = relationType;
			ToEntityName = toEntityName;
			MappedByPropertyName = mappedByPropertyName;
			IdMapper = idMapper;
			FakeBidirectionalRelationMapper = fakeBidirectionalRelationMapper;
			FakeBidirectionalRelationIndexMapper = fakeBidirectionalRelationIndexMapper;
			Insertable = insertable;
			IsIgnoreNotFound = ignoreNotFound;

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
		public bool IsIgnoreNotFound { get; private set; }
		public bool Bidirectional { get; set; }
	}
}

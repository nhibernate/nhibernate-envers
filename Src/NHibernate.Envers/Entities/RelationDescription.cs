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
			                               ignoreNotFound, false);
		}

		public static RelationDescription ToMany(string fromPropertyName, RelationType relationType, string toEntityName,
		                                         string mappedByPropertyName, IIdMapper idMapper,
		                                         IPropertyMapper fakeBidirectionalRelationMapper,
		                                         IPropertyMapper fakeBidirectionalRelationIndexMapper, 
												bool insertable, bool indexed)
		{
			// Envers populates collections by executing dedicated queries. Special handling of
			// @NotFound(action = NotFoundAction.IGNORE) can be omitted in such case as exceptions
			// (e.g. EntityNotFoundException, ObjectNotFoundException) are never thrown.
			// Therefore assigning false to ignoreNotFound.
			return new RelationDescription(fromPropertyName, relationType, toEntityName, mappedByPropertyName, idMapper,
															 fakeBidirectionalRelationMapper, fakeBidirectionalRelationIndexMapper, insertable,
															 false, indexed);
		}

		private RelationDescription(string fromPropertyName, RelationType relationType, string toEntityName,
						   string mappedByPropertyName, IIdMapper idMapper,
						   IPropertyMapper fakeBidirectionalRelationMapper,
						   IPropertyMapper fakeBidirectionalRelationIndexMapper, bool insertable, 
							bool ignoreNotFound, bool indexed)
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
			IsIndexed = indexed;
			Bidirectional = false;
		}

		public string FromPropertyName { get; }
		public RelationType RelationType { get; }
		public string ToEntityName { get; }
		public string MappedByPropertyName { get; }
		public IIdMapper IdMapper { get; }
		public IPropertyMapper FakeBidirectionalRelationMapper { get; }
		public IPropertyMapper FakeBidirectionalRelationIndexMapper { get; }
		public bool Insertable { get; }
		public bool IsIgnoreNotFound { get; }
		public bool Bidirectional { get; set; }
		public bool IsIndexed { get; }
	}
}

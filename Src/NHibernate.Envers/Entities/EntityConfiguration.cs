using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Id;

namespace NHibernate.Envers.Entities
{
	[Serializable]
	public class EntityConfiguration
	{
		public EntityConfiguration(string versionsEntityName, string entityClassName, IdMappingData idMappingData,
							IExtendedPropertyMapper propertyMapper, string parentEntityName, Func<System.Type, object> factory)
		{
			VersionsEntityName = versionsEntityName;
			EntityClassName = entityClassName;
			IdMappingData = idMappingData;
			PropertyMapper = propertyMapper;
			ParentEntityName = parentEntityName;

			Factory = factory;
			relations = new Dictionary<string, RelationDescription>();
		}
	
		public string VersionsEntityName { get; }
		public string EntityClassName { get; }
		public IdMappingData IdMappingData { get; }
		public IExtendedPropertyMapper PropertyMapper { get; }
		// Maps from property name
		private readonly IDictionary<string, RelationDescription> relations;
		public string ParentEntityName { get; }

		public IIdMapper IdMapper => IdMappingData.IdMapper;

		public IEnumerable<RelationDescription> RelationsIterator => relations.Values;

		public void AddToOneRelation(string fromPropertyName, string toEntityName, IIdMapper idMapper, bool insertable, bool ignoreNotFound)
		{
			relations.Add(fromPropertyName, RelationDescription.ToOne(fromPropertyName, RelationType.ToOne,
					toEntityName, null, idMapper, null, null, insertable, ignoreNotFound));
		}

		public void AddToOneNotOwningRelation(string fromPropertyName, string mappedByPropertyName, string toEntityName,
											  IIdMapper idMapper, bool ignoreNotFound)
		{
			relations.Add(fromPropertyName, RelationDescription.ToOne(fromPropertyName, RelationType.ToOneNotOwning,
					toEntityName, mappedByPropertyName, idMapper, null, null, true, ignoreNotFound));
		}

		public void AddToManyNotOwningRelation(string fromPropertyName, string mappedByPropertyName, string toEntityName,
											   IIdMapper idMapper, IPropertyMapper fakeBidirectionalRelationMapper,
											   IPropertyMapper fakeBidirectionalRelationIndexMapper, RelationType relationType,
												bool indexed)
		{
			relations.Add(fromPropertyName, RelationDescription.ToMany(fromPropertyName, relationType,
					toEntityName, mappedByPropertyName, idMapper, fakeBidirectionalRelationMapper,
					fakeBidirectionalRelationIndexMapper, true, indexed));
		}

		public void AddToManyMiddleRelation(string fromPropertyName, string toEntityName)
		{
			relations.Add(fromPropertyName, RelationDescription.ToMany(fromPropertyName, RelationType.ToManyMiddle,
					toEntityName, null, null, null, null, true, false));
		}

		public void AddToManyMiddleNotOwningRelation(string fromPropertyName, string mappedByPropertyName, string toEntityName)
		{
			relations.Add(fromPropertyName, RelationDescription.ToMany(fromPropertyName, RelationType.ToManyMiddleNotOwning,
					toEntityName, mappedByPropertyName, null, null, null, true, false));
		}

		public bool IsRelation(string propertyName)
		{
			return relations.ContainsKey(propertyName) && relations[propertyName] != null;
		}

		public RelationDescription GetRelationDescription(string propertyName)
		{
			return relations.ContainsKey(propertyName)? relations[propertyName]: null;
		}

		public Func<System.Type, object> Factory { get; }
	}
}

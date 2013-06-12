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
	
		public string VersionsEntityName { get; private set; }
		public string EntityClassName { get; private set; }
		public IdMappingData IdMappingData { get; private set; }
		public IExtendedPropertyMapper PropertyMapper { get; private set; }
		// Maps from property name
		private readonly IDictionary<string, RelationDescription> relations;
		public string ParentEntityName { get; private set; }

		public IIdMapper IdMapper
		{
			get { return IdMappingData.IdMapper; }
		}


		public IEnumerable<RelationDescription> RelationsIterator
		{
			get { return relations.Values; }
		}

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
											   IPropertyMapper fakeBidirectionalRelationIndexMapper)
		{
			relations.Add(fromPropertyName, RelationDescription.ToMany(fromPropertyName, RelationType.ToManyNotOwning,
					toEntityName, mappedByPropertyName, idMapper, fakeBidirectionalRelationMapper,
					fakeBidirectionalRelationIndexMapper, true));
		}

		public void AddToManyMiddleRelation(string fromPropertyName, string toEntityName)
		{
			relations.Add(fromPropertyName, RelationDescription.ToMany(fromPropertyName, RelationType.ToManyMiddle,
					toEntityName, null, null, null, null, true));
		}

		public void AddToManyMiddleNotOwningRelation(string fromPropertyName, string mappedByPropertyName, string toEntityName)
		{
			relations.Add(fromPropertyName, RelationDescription.ToMany(fromPropertyName, RelationType.ToManyMiddleNotOwning,
					toEntityName, mappedByPropertyName, null, null, null, true));
		}

		public bool IsRelation(string propertyName)
		{
			return relations.ContainsKey(propertyName) && relations[propertyName] != null;
		}

		public RelationDescription GetRelationDescription(string propertyName)
		{
			return relations.ContainsKey(propertyName)? relations[propertyName]: null;
		}

		public Func<System.Type, object> Factory { get; private set; }
	}
}

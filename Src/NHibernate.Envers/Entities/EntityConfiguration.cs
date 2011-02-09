using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Entities.Mapper;

namespace NHibernate.Envers.Entities
{
    public class EntityConfiguration
    {
        public string VersionsEntityName { get; private set; }
        public IdMappingData IdMappingData { get; private set; }
        public IExtendedPropertyMapper PropertyMapper { get; private set; }
        // Maps from property name
        private readonly IDictionary<string, RelationDescription> relations;
        public string ParentEntityName { get; private set; }

        public EntityConfiguration(string versionsEntityName, IdMappingData idMappingData,
                                   IExtendedPropertyMapper propertyMapper, string parentEntityName)
        {
            VersionsEntityName = versionsEntityName;
            IdMappingData = idMappingData;
            PropertyMapper = propertyMapper;
            ParentEntityName = parentEntityName;

			relations = new Dictionary<string, RelationDescription>();
        }

        public void AddToOneRelation(string fromPropertyName, string toEntityName, IIdMapper idMapper, bool insertable)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.ToOne,
                    toEntityName, null, idMapper, null, null, insertable));
        }

        public void AddToOneNotOwningRelation(string fromPropertyName, string mappedByPropertyName, string toEntityName,
                                              IIdMapper idMapper)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.ToOneNotOwning,
                    toEntityName, mappedByPropertyName, idMapper, null, null, true));
        }

        public void AddToManyNotOwningRelation(string fromPropertyName, string mappedByPropertyName, string toEntityName,
                                               IIdMapper idMapper, IPropertyMapper fakeBidirectionalRelationMapper,
                                               IPropertyMapper fakeBidirectionalRelationIndexMapper)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.ToManyNotOwning,
                    toEntityName, mappedByPropertyName, idMapper, fakeBidirectionalRelationMapper,
                    fakeBidirectionalRelationIndexMapper, true));
        }

        public void addToManyMiddleRelation(string fromPropertyName, string toEntityName)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.ToManyMiddle,
                    toEntityName, null, null, null, null, true));
        }

        public void AddToManyMiddleNotOwningRelation(string fromPropertyName, string mappedByPropertyName, string toEntityName)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.ToManyMiddleNotOwning,
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

        // For use by EntitiesConfigurations

        public IIdMapper GetIdMapper()
        {
            return IdMappingData.IdMapper;
        }

        //TODO Simon rename - getRelationsCollection
        public ICollection<RelationDescription> GetRelationsIterator()
        {
            return relations.Values;
        }
    }
}

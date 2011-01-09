using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Entities.Mapper;

namespace NHibernate.Envers.Entities
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class EntityConfiguration
    {
        public String VersionsEntityName { get; private set; }
        public IdMappingData IdMappingData { get; private set; }
        public IExtendedPropertyMapper PropertyMapper { get; private set; }
        // Maps from property name
        private IDictionary<String, RelationDescription> relations;
        public String ParentEntityName { get; private set; }

        public EntityConfiguration(String versionsEntityName, IdMappingData idMappingData,
                                   IExtendedPropertyMapper propertyMapper, String parentEntityName)
        {
            this.VersionsEntityName = versionsEntityName;
            this.IdMappingData = idMappingData;
            this.PropertyMapper = propertyMapper;
            this.ParentEntityName = parentEntityName;

            this.relations = new Dictionary<String, RelationDescription>();
        }

        public void AddToOneRelation(String fromPropertyName, String toEntityName, IIdMapper idMapper, bool insertable)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.TO_ONE,
                    toEntityName, null, idMapper, null, null, insertable));
        }

        public void AddToOneNotOwningRelation(String fromPropertyName, String mappedByPropertyName, String toEntityName,
                                              IIdMapper idMapper)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.TO_ONE_NOT_OWNING,
                    toEntityName, mappedByPropertyName, idMapper, null, null, true));
        }

        public void AddToManyNotOwningRelation(String fromPropertyName, String mappedByPropertyName, String toEntityName,
                                               IIdMapper idMapper, IPropertyMapper fakeBidirectionalRelationMapper,
                                               IPropertyMapper fakeBidirectionalRelationIndexMapper)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.TO_MANY_NOT_OWNING,
                    toEntityName, mappedByPropertyName, idMapper, fakeBidirectionalRelationMapper,
                    fakeBidirectionalRelationIndexMapper, true));
        }

        public void addToManyMiddleRelation(String fromPropertyName, String toEntityName)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.TO_MANY_MIDDLE,
                    toEntityName, null, null, null, null, true));
        }

        public void AddToManyMiddleNotOwningRelation(String fromPropertyName, String mappedByPropertyName, String toEntityName)
        {
            relations.Add(fromPropertyName, new RelationDescription(fromPropertyName, RelationType.TO_MANY_MIDDLE_NOT_OWNING,
                    toEntityName, mappedByPropertyName, null, null, null, true));
        }

        public bool IsRelation(String propertyName)
        {
            return relations.ContainsKey(propertyName) && relations[propertyName] != null;
        }

        public RelationDescription GetRelationDescription(String propertyName)
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

using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Entities.Mapper;

namespace NHibernate.Envers.Entities
{
    public class RelationDescription {
        public virtual String FromPropertyName { get; private set;}
        public virtual RelationType RelationType { get; private set; }
        public virtual String ToEntityName { get; private set; }
        public virtual String MappedByPropertyName { get; private set; }
        public virtual IIdMapper IdMapper { get; private set; }
        public virtual IPropertyMapper FakeBidirectionalRelationMapper { get; private set; }
        public virtual IPropertyMapper FakeBidirectionalRelationIndexMapper { get; private set; }
        public virtual bool Insertable { get; private set; }
        public bool Bidirectional { get; set; }

        public RelationDescription(String fromPropertyName, RelationType relationType, String toEntityName,
                                   String mappedByPropertyName, IIdMapper idMapper,
                                   IPropertyMapper fakeBidirectionalRelationMapper,
                                   IPropertyMapper fakeBidirectionalRelationIndexMapper, bool insertable) {
            this.FromPropertyName = fromPropertyName;
            this.RelationType = relationType;
            this.ToEntityName = toEntityName;
            this.MappedByPropertyName = mappedByPropertyName;
            this.IdMapper = idMapper;
            this.FakeBidirectionalRelationMapper = fakeBidirectionalRelationMapper;
            this.FakeBidirectionalRelationIndexMapper = fakeBidirectionalRelationIndexMapper;
            this.Insertable = insertable;

            this.Bidirectional = false;
        }

    }
}

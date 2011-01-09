using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Configuration;


namespace NHibernate.Envers.Entities.Mapper.Relation
{
    /**
     * A class holding information about ids, which form a virtual "relation" from a middle-table. Middle-tables are used
     * when mapping collections.
     * @author Adam Warski (adam at warski dot org)
     */
    public sealed class MiddleIdData {
        public IIdMapper OriginalMapper { get; private set; }
        public IIdMapper PrefixedMapper { get; private set; }
        public String EntityName { get; private set; }
        public String AuditEntityName { get; private set; }

        public MiddleIdData(AuditEntitiesConfiguration verEntCfg, IdMappingData mappingData, String prefix,
                            String entityName, bool audited) {
            this.OriginalMapper = mappingData.IdMapper;
            this.PrefixedMapper = mappingData.IdMapper.PrefixMappedProperties(prefix);
            this.EntityName = entityName;
            this.AuditEntityName = audited ? verEntCfg.GetAuditEntityName(entityName) : null;
        }

        /**
         * @return Is the entity, to which this middle id data correspond, audited.
         */
        public bool IsAudited() {
            return AuditEntityName != null;
        }
    }
}

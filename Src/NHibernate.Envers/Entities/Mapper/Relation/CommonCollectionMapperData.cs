using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
    /**
     * Data that is used by all collection mappers, regardless of the type.  
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public sealed class CommonCollectionMapperData {

        private readonly AuditEntitiesConfiguration _verEntCfg;
        public AuditEntitiesConfiguration VerEntCfg { get { return _verEntCfg; } }
        private readonly String _versionsMiddleEntityName;
        public String VersionsMiddleEntityName { get { return _versionsMiddleEntityName; } }
        private readonly PropertyData _collectionReferencingPropertyData;
        public PropertyData CollectionReferencingPropertyData { get { return _collectionReferencingPropertyData; } }
        private readonly MiddleIdData _referencingIdData;
        public MiddleIdData ReferencingIdData { get { return _referencingIdData; } }
        private readonly IRelationQueryGenerator _queryGenerator;
        public IRelationQueryGenerator QueryGenerator { get { return _queryGenerator; } }

        public CommonCollectionMapperData(AuditEntitiesConfiguration verEntCfg, String versionsMiddleEntityName,
                                          PropertyData collectionReferencingPropertyData, MiddleIdData referencingIdData,
                                          IRelationQueryGenerator queryGenerator) {
            this._verEntCfg = verEntCfg;
            this._versionsMiddleEntityName = versionsMiddleEntityName;
            this._collectionReferencingPropertyData = collectionReferencingPropertyData;
            this._referencingIdData = referencingIdData;
            this._queryGenerator = queryGenerator;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Exceptions;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class CriteriaTools {
        private CriteriaTools() { }

        public static void CheckPropertyNotARelation(AuditConfiguration verCfg, String entityName,
                                                     String propertyName){
            if (verCfg.EntCfg[entityName].IsRelation(propertyName)) {
                throw new AuditException("This criterion cannot be used on a property that is " +
                        "a relation to another property.");
            }
        }

        public static RelationDescription GetRelatedEntity(AuditConfiguration verCfg, String entityName,
                                                           String propertyName){
            RelationDescription relationDesc = verCfg.EntCfg.GetRelationDescription(entityName, propertyName);

            if (relationDesc == null) {
                return null;
            }

            if (relationDesc.RelationType == RelationType.TO_ONE) {
                return relationDesc;
            }

            throw new AuditException("This type of relation (" + entityName + "." + propertyName +
                    ") isn't supported and can't be used in queries.");
        }
    }
}

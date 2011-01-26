using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using log4net;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Util;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Configuration
{
    /**
     * A helper class holding auditing meta-data for all persistent classes.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class ClassesAuditingData {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClassesAuditingData));

        private readonly IDictionary<String, ClassAuditingData> entityNameToAuditingData = new Dictionary<String, ClassAuditingData>();
        //Simon 27/06/2010 - era new LinkedHashMap...
        private readonly IDictionary<PersistentClass, ClassAuditingData> persistentClassToAuditingData = new Dictionary<PersistentClass, ClassAuditingData>();

        /**
         * Stores information about auditing meta-data for the given class.
         * @param pc Persistent class.
         * @param cad Auditing meta-data for the given class.
         */
        public void AddClassAuditingData(PersistentClass pc, ClassAuditingData cad) {
            entityNameToAuditingData.Add(pc.EntityName, cad);
            persistentClassToAuditingData.Add(pc, cad);
        }

        /**
         * @return A collection of all auditing meta-data for persistent classes.
         */
        public ICollection<KeyValuePair<PersistentClass, ClassAuditingData>> GetAllClassAuditedData() {
            return persistentClassToAuditingData;
        }

        /**
         * @param entityName Name of the entity.
         * @return Auditing meta-data for the given entity.
         */
        public ClassAuditingData GetClassAuditingData(String entityName) {
            return entityNameToAuditingData[entityName];
        }

        /**
         * After all meta-data is read, updates calculated fields. This includes:
         * <ul>
         * <li>setting {@code forceInsertable} to {@code true} for properties specified by {@code @AuditMappedBy}</li> 
         * </ul>
         */
        public void UpdateCalculatedFields() {
            foreach (KeyValuePair<PersistentClass, ClassAuditingData> classAuditingDataEntry in persistentClassToAuditingData) {
                PersistentClass pc = classAuditingDataEntry.Key;
                ClassAuditingData classAuditingData = classAuditingDataEntry.Value;
                foreach (String propertyName in classAuditingData.PropertyNames) {
                    PropertyAuditingData propertyAuditingData = classAuditingData.GetPropertyAuditingData(propertyName);
                    // If a property had the @AuditMappedBy annotation, setting the referenced fields to be always insertable.
                    if (propertyAuditingData.AuditMappedBy != null) {
                        String referencedEntityName = MappingTools.getReferencedEntityName(pc.GetProperty(propertyName).Value);

                        ClassAuditingData referencedClassAuditingData = entityNameToAuditingData[referencedEntityName];

                        ForcePropertyInsertable(referencedClassAuditingData, propertyAuditingData.AuditMappedBy,
                                pc.EntityName, referencedEntityName);

                        ForcePropertyInsertable(referencedClassAuditingData, propertyAuditingData.PositionMappedBy,
                                pc.EntityName, referencedEntityName);
                    }
                }
            }
        }

        private static void ForcePropertyInsertable(ClassAuditingData classAuditingData, String propertyName,
                                             String entityName, String referencedEntityName) {
            if (propertyName != null) {
                if (classAuditingData.GetPropertyAuditingData(propertyName) == null) {
                    throw new MappingException("@AuditMappedBy points to a property that doesn't exist: " +
                        referencedEntityName + "." + propertyName);
                }

                log.Debug("Non-insertable property " + referencedEntityName + "." + propertyName +
                        " will be made insertable because a matching @AuditMappedBy was found in the " +
                        entityName + " entity.");

                classAuditingData
                        .GetPropertyAuditingData(propertyName)
                        .ForceInsertable = true;
            }
        }
    }
}

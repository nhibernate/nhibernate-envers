using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Engine;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Collection;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
    public class ToOneIdMapper : IPropertyMapper
    {
        private IIdMapper delegat;
        private PropertyData propertyData;
        private string referencedEntityName;
        private bool nonInsertableFake;

        public ToOneIdMapper(IIdMapper delegat, PropertyData propertyData, string referencedEntityName, bool nonInsertableFake)
        {
            this.delegat = delegat;
            this.propertyData = propertyData;
            this.referencedEntityName = referencedEntityName;
            this.nonInsertableFake = nonInsertableFake;
        }

        public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
        {
            //Simon 27/06/2010 - era new LinkedHashMap
            var newData = new Dictionary<String, Object>();
            data.Add(propertyData.Name, newData);

            // If this property is originally non-insertable, but made insertable because it is in a many-to-one "fake"
            // bi-directional relation, we always store the "old", unchaged data, to prevent storing changes made
            // to this field. It is the responsibility of the collection to properly update it if it really changed.
            delegat.MapToMapFromEntity(newData, nonInsertableFake ? oldObj : newObj);

            //noinspection SimplifiableConditionalExpression
            return nonInsertableFake ? false : !Toolz.EntitiesEqual(session, newObj, oldObj);
        }

        public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary<string, object> data, object primaryKey,
                                       IAuditReaderImplementor versionsReader, long revision)
        {
            if (obj == null)
            {
                return;
            }

            object entityId = delegat.MapToIdFromMap(DictionaryWrapper<string, object>.Wrap((IDictionary)data[propertyData.Name]));
            object value;
            if (entityId == null)
            {
                value = null;
            }
            else
            {
                if (versionsReader.FirstLevelCache.Contains(referencedEntityName, revision, entityId))
                {
                    value = versionsReader.FirstLevelCache[referencedEntityName, revision, entityId];
                }
                else
                {
                    //java: Class<?> entityClass = ReflectionTools.loadClass(referencedEntityName); 
                    value = versionsReader.SessionImplementor.Factory.GetEntityPersister(referencedEntityName).CreateProxy(
                        entityId, new ToOneDelegateSessionImplementor(versionsReader, Toolz.ResolveDotnetType(referencedEntityName), entityId, revision, verCfg));
                }
            }
        	var setter = ReflectionTools.GetSetter(obj.GetType(), propertyData);
			setter.Set(obj, value);
        }

        public IList<PersistentCollectionChangeData> MapCollectionChanges(string referencingPropertyName,
                                                                         IPersistentCollection newColl,
                                                                         object oldColl,
                                                                         object id)
        {
            return null;
        }
    }
}

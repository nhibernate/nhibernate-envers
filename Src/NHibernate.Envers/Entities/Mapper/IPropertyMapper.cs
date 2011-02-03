using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Collection;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public interface IPropertyMapper
    {
        /**
         * Maps properties to the given map, basing on differences between properties of new and old objects.
         * @param session The current session.
         * @param data Data to map to.
         * @param newObj New state of the entity.
         * @param oldObj Old state of the entity.
         * @return True if there are any differences between the states represented by newObj and oldObj.
         */
        bool MapToMapFromEntity(ISessionImplementor session, IDictionary<String, Object> data, Object newObj, Object oldObj);

        /**
         * Maps properties from the given map to the given object.
         * @param verCfg Versions configuration.
         * @param obj Object to map to.
         * @param data Data to map from.
         * @param primaryKey Primary key of the object to which we map (for relations)
         * @param versionsReader VersionsReader for reading relations
         * @param revision Revision at which the object is read, for reading relations
         */
        void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey,
                                IAuditReaderImplementor versionsReader, long revision);

        /**
         * Maps collection changes
         * @param referencingPropertyName Name of the field, which holds the collection in the entity.
         * @param newColl New collection, after updates.
         * @param oldColl Old collection, before updates.
         * @param id Id of the object owning the collection.
         * @return List of changes that need to be performed on the persistent store.
         */
        IList<PersistentCollectionChangeData> MapCollectionChanges(String referencingPropertyName,
                                                                  IPersistentCollection newColl,
                                                                  Object oldColl, Object id);
    }
}

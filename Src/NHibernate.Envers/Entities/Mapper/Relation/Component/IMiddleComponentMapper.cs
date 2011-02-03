using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public interface IMiddleComponentMapper
    {
        /**
         * Maps from full object data, contained in the given map (or object representation of the map, if
         * available), to an object.
         * @param entityInstantiator An entity instatiator bound with an open versions reader.
         * @param data Full object data.
         * @param dataObject An optional object representation of the data.
         * @param revision Revision at which the data is read.
         * @return An object with data corresponding to the one found in the given map.
         */
        object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary data, object dataObject, long revision);

        /**
         * Maps from an object to the object's map representation (for an entity - only its id).
         * @param data Map to which data should be added.
         * @param obj Object to map from.
         */
        void MapToMapFromObject(IDictionary<string, Object> data, Object obj);

        /**
         * Adds query statements, which contains restrictions, which express the property that part of the middle
         * entity with alias prefix1, is equal to part of the middle entity with alias prefix2 (the entity is the same).
         * The part is the component's representation in the middle entity.
         * @param parameters Parameters, to which to add the statements.
         * @param prefix1 First alias of the entity + prefix to add to the properties.
         * @param prefix2 Second alias of the entity + prefix to add to the properties.
         */
        void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2);
    }
}

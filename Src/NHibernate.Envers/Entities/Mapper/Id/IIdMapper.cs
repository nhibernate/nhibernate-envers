using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Id
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public interface IIdMapper
    {
        void MapToMapFromId(IDictionary<String, Object> data, Object obj);

        void MapToMapFromEntity(IDictionary<String, Object> data, Object obj);

        void MapToEntityFromMap(Object obj, IDictionary<string,object> data);

        Object MapToIdFromEntity(Object data);

        Object MapToIdFromMap(IDictionary<string,object> data);

        /**
         * Creates a mapper with all mapped properties prefixed. A mapped property is a property which
         * is directly mapped to values (not composite).
         * @param prefix Prefix to add to mapped properties
         * @return A copy of the current property mapper, with mapped properties prefixed.
         */
        IIdMapper PrefixMappedProperties(String prefix);

        /**
         * @param obj Id from which to map.
         * @return A set parameter data, needed to build a query basing on the given id.
         */
        IList<QueryParameterData> MapToQueryParametersFromId(Object obj);

        /**
         * Adds query statements, which contains restrictions, which express the property that the id of the entity
         * with alias prefix1, is equal to the id of the entity with alias prefix2 (the entity is the same).
         * @param parameters Parameters, to which to add the statements.
         * @param prefix1 First alias of the entity + prefix to add to the properties.
         * @param prefix2 Second alias of the entity + prefix to add to the properties.
         */
        void AddIdsEqualToQuery(Parameters parameters, String prefix1, String prefix2);

        /**
         * Adds query statements, which contains restrictions, which express the property that the id of the entity
         * with alias prefix1, is equal to the id of the entity with alias prefix2 mapped by the second mapper
         * (the second mapper must be for the same entity, but it can have, for example, prefixed properties).
         * @param parameters Parameters, to which to add the statements.
         * @param prefix1 First alias of the entity + prefix to add to the properties.
         * @param mapper2 Second mapper for the same entity, which will be used to get properties for the right side
         * of the equation.
         * @param prefix2 Second alias of the entity + prefix to add to the properties.
         */
        void AddIdsEqualToQuery(Parameters parameters, String prefix1, IIdMapper mapper2, String prefix2);

        /**
         * Adds query statements, which contains restrictions, which express the property that the id of the entity
         * with alias prefix, is equal to the given object.
         * @param parameters Parameters, to which to add the statements.
         * @param id Value of id.
         * @param prefix Prefix to add to the properties (may be null).
         * @param equals Should this query express the "=" relation or the "<>" relation.
         */
        void AddIdEqualsToQuery(Parameters parameters, Object id, String prefix, bool equals);

        /**
         * Adds query statements, which contains named parameters, which express the property that the id of the entity
         * with alias prefix, is equal to the given object. It is the responsibility of the using method to read
         * parameter values from the id and specify them on the final query object.
         * @param parameters Parameters, to which to add the statements.
         * @param prefix Prefix to add to the properties (may be null).
         * @param equals Should this query express the "=" relation or the "<>" relation.
         */
        void AddNamedIdEqualsToQuery(Parameters parameters, String prefix, bool equals);
    }
}

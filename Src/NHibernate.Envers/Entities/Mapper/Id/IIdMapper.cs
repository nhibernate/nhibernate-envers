using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Id
{
    public interface IIdMapper
    {
        void MapToMapFromId(IDictionary<string, object> data, object obj);
        void MapToMapFromEntity(IDictionary<string, object> data, object obj);
        void MapToEntityFromMap(object obj, IDictionary data);
        object MapToIdFromEntity(object data);
        object MapToIdFromMap(IDictionary data);

        /**
         * Creates a mapper with all mapped properties prefixed. A mapped property is a property which
         * is directly mapped to values (not composite).
         * @param prefix Prefix to add to mapped properties
         * @return A copy of the current property mapper, with mapped properties prefixed.
         */
        IIdMapper PrefixMappedProperties(string prefix);

        /**
         * @param obj Id from which to map.
         * @return A set parameter data, needed to build a query basing on the given id.
         */
        IList<QueryParameterData> MapToQueryParametersFromId(object obj);

        /**
         * Adds query statements, which contains restrictions, which express the property that the id of the entity
         * with alias prefix1, is equal to the id of the entity with alias prefix2 (the entity is the same).
         * @param parameters Parameters, to which to add the statements.
         * @param prefix1 First alias of the entity + prefix to add to the properties.
         * @param prefix2 Second alias of the entity + prefix to add to the properties.
         */
        void AddIdsEqualToQuery(Parameters parameters, string prefix1, string prefix2);

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
        void AddIdsEqualToQuery(Parameters parameters, string prefix1, IIdMapper mapper2, string prefix2);

        /**
         * Adds query statements, which contains restrictions, which express the property that the id of the entity
         * with alias prefix, is equal to the given object.
         * @param parameters Parameters, to which to add the statements.
         * @param id Value of id.
         * @param prefix Prefix to add to the properties (may be null).
         * @param equals Should this query express the "=" relation or the "<>" relation.
         */
        void AddIdEqualsToQuery(Parameters parameters, object id, string prefix, bool equals);

        /**
         * Adds query statements, which contains named parameters, which express the property that the id of the entity
         * with alias prefix, is equal to the given object. It is the responsibility of the using method to read
         * parameter values from the id and specify them on the final query object.
         * @param parameters Parameters, to which to add the statements.
         * @param prefix Prefix to add to the properties (may be null).
         * @param equals Should this query express the "=" relation or the "<>" relation.
         */
        void AddNamedIdEqualsToQuery(Parameters parameters, string prefix, bool equals);
    }
}

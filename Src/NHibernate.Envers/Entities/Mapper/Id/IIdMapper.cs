using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Id
{
	public interface IIdMapper
	{
		void MapToMapFromId(IDictionary<string, object> data, object obj);
		void MapToMapFromEntity(IDictionary<string, object> data, object obj);
		/// <summary>
		/// </summary>
		/// <param name="obj">Object to map to.</param>
		/// <param name="data">Data to map.</param>
		/// <returns>True if data was mapped; false otherwise (when the id is <code>null</code>).</returns>
		bool MapToEntityFromMap(object obj, IDictionary data);
		object MapToIdFromEntity(object data);
		object MapToIdFromMap(IDictionary data);

		/// <summary>
		///  Creates a mapper with all mapped properties prefixed. A mapped property is a property which
		///  is directly mapped to values (not composite).
		/// </summary>
		/// <param name="prefix">Prefix to add to mapped properties</param>
		/// <returns>A copy of the current property mapper, with mapped properties prefixed.</returns>
		IIdMapper PrefixMappedProperties(string prefix);
		
		/// <summary>
		/// </summary>
		/// <param name="obj">Id from which to map.</param>
		/// <returns>A set parameter data, needed to build a query basing on the given id.</returns>
		IList<QueryParameterData> MapToQueryParametersFromId(object obj);

		/// <summary>
		/// Adds query statements, which contains restrictions, which express the property that the id of the entity
		/// with alias prefix1, is equal to the id of the entity with alias prefix2 (the entity is the same).
		/// </summary>
		/// <param name="parameters">Parameters, to which to add the statements.</param>
		/// <param name="prefix1">First alias of the entity + prefix to add to the properties.</param>
		/// <param name="prefix2">Second alias of the entity + prefix to add to the properties.</param>
		void AddIdsEqualToQuery(Parameters parameters, string prefix1, string prefix2);

		/// <summary>
		/// Adds query statements, which contains restrictions, which express the property that the id of the entity
		/// with alias prefix1, is equal to the id of the entity with alias prefix2 mapped by the second mapper
		/// (the second mapper must be for the same entity, but it can have, for example, prefixed properties).
		/// </summary>
		/// <param name="parameters">Parameters, to which to add the statements.</param>
		/// <param name="prefix1">First alias of the entity + prefix to add to the properties.</param>
		/// <param name="mapper2">Second mapper for the same entity, which will be used to get properties for the right side of the equation.</param>
		/// <param name="prefix2">Second alias of the entity + prefix to add to the properties.</param>
		void AddIdsEqualToQuery(Parameters parameters, string prefix1, IIdMapper mapper2, string prefix2);

		/// <summary>
		/// Adds query statements, which contains restrictions, which express the property that the id of the entity with alias prefix, is equal to the given object.
		/// </summary>
		/// <param name="parameters">Parameters, to which to add the statements.</param>
		/// <param name="id">Value of id.</param>
		/// <param name="prefix">Prefix to add to the properties (may be null).</param>
		/// <param name="equals">Should this query express the "=" relation or the "!=" relation.</param>
		void AddIdEqualsToQuery(Parameters parameters, object id, string prefix, bool equals);

		/// <summary>
		/// Adds query statements, which contains named parameters, which express the property that the id of the entity with alias prefix, is equal to the given object.
		/// </summary>
		/// <param name="parameters">Parameters, to which to add the statements.</param>
		/// <param name="prefix">Prefix to add to the properties (may be null).</param>
		/// <param name="equals">Should this query express the "=" relation or the "!=" relation.</param>
		/// <remarks>It is the responsibility of the using method to read parameter values from the id and specify them on the final query object.</remarks>
		void AddNamedIdEqualsToQuery(Parameters parameters, string prefix, bool equals);
	}
}

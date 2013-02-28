using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	public interface IMiddleComponentMapper
	{
		/// <summary>
		/// Maps from full object data, contained in the given map (or object representation of the map, if
		///  available), to an object.
		/// </summary>
		/// <param name="entityInstantiator">An entity instatiator bound with an open versions reader.</param>
		/// <param name="data">Full object data.</param>
		/// <param name="dataObject">An optional object representation of the data.</param>
		/// <param name="revision">Revision at which the data is read.</param>
		/// <returns>An object with data corresponding to the one found in the given map.</returns>
		object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary data, object dataObject, long revision);

		/// <summary>
		/// Maps from an object to the object's map representation (for an entity - only its id).
		/// </summary>
		/// <param name="idData">Map to which composite-id data should be added.</param>
		/// <param name="data">Map to which data should be added.</param>
		/// <param name="obj">Object to map from.</param>
		/// <param name="session">The current session</param>
		void MapToMapFromObject(ISessionImplementor session, IDictionary<String, Object> idData, IDictionary<string, Object> data, Object obj);

		/// <summary>
		/// Adds query statements, which contains restrictions, which express the property that part of the middle
		/// entity with alias prefix1, is equal to part of the middle entity with alias prefix2 (the entity is the same).
		/// The part is the component's representation in the middle entity.
		/// </summary>
		/// <param name="parameters">Parameters, to which to add the statements.</param>
		/// <param name="idPrefix1">First alias of the entity + prefix + id to add to the properties.</param>
		/// <param name="prefix1">First alias of the entity + prefix to add to the properties.</param>
		/// <param name="idPrefix2">Second alias of the entity + prefix + id to add to the properties.</param>
		/// <param name="prefix2">Second alias of the entity + prefix to add to the properties.</param>
		void AddMiddleEqualToQuery(Parameters parameters, string idPrefix1, string prefix1, string idPrefix2, string prefix2);
	}
}

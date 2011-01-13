using System;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	/**
	 * A mapper for reading and writing a property straight to/from maps. This mapper cannot be used with middle tables,
	 * but only with "fake" bidirectional indexed relations. 
	 * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
	 */
	public sealed class MiddleStraightComponentMapper : IMiddleComponentMapper 
	{
		private readonly string _propertyName;

		public MiddleStraightComponentMapper(string propertyName) 
		{
			_propertyName = propertyName;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary<string, object> data,
											 object dataObject, long revision)
		{
			object ret;
			return data.TryGetValue(_propertyName, out ret) ? ret : null;
		}

		public void MapToMapFromObject(IDictionary<string, object> data, object obj) 
		{
			data.Add(_propertyName, obj);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2) 
		{
			throw new NotSupportedException("Cannot use this mapper with a middle table!");
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	/// <summary>
	/// A mapper for reading and writing a property straight to/from maps. This mapper cannot be used with middle tables,
	/// but only with "fake" bidirectional indexed relations. 
	/// </summary>
	[Serializable]
	public sealed class MiddleStraightComponentMapper : IMiddleComponentMapper 
	{
		private readonly string _propertyName;

		public MiddleStraightComponentMapper(string propertyName) 
		{
			_propertyName = propertyName;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary data, object dataObject, long revision)
		{
			return data.Contains(_propertyName) ? data[_propertyName] : null;
		}

		public void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object obj)
		{
			idData.Add(_propertyName, obj);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string idPrefix1, string prefix1, string idPrefix2, string prefix2)
		{
			throw new NotSupportedException("Cannot use this mapper with a middle table!");
		}
	}
}

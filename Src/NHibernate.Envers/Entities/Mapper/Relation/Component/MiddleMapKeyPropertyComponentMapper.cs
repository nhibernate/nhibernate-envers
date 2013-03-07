using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Tools.Query;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	/// <summary>
	/// A component mapper for the @MapKey mapping with the name parameter specified: the value of the map's key
	/// is a property of the entity. This doesn't have an effect on the data stored in the versions tables,
	/// so <code>mapToMapFromObject</code> is empty.
	/// </summary>
	[Serializable]
	public class MiddleMapKeyPropertyComponentMapper : IMiddleComponentMapper 
	{
		private readonly string _propertyName;
		private readonly string _accessType;

		public MiddleMapKeyPropertyComponentMapper(string propertyName, string accessType) 
		{
			_propertyName = propertyName;
			_accessType = accessType;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, 
											IDictionary data,
											object dataObject, 
											long revision)
		{
			// dataObject is not null, as this mapper can only be used in an index.
			return ReflectionTools.GetGetter(dataObject.GetType(), _propertyName, _accessType).Get(dataObject);
		}

		public void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object obj)
		{
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string idPrefix1, string prefix1, string idPrefix2, string prefix2)
		{
		}
	}
}

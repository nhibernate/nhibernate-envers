using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Query;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	/// <summary>
	/// A component mapper for the @MapKey mapping with the name parameter specified: the value of the map's key
	/// is a property of the entity. This doesn't have an effect on the data stored in the versions tables,
	/// so <code>mapToMapFromObject</code> is empty.
	/// </summary>
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

		public void MapToMapFromObject(IDictionary<string, object> data, object obj) 
		{
			// Doing nothing.
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2) 
		{
			// Doing nothing.
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	[Serializable]
	public sealed class MiddleDummyComponentMapper : IMiddleComponentMapper
	{
		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary data, object dataObject, long revision)
		{
			return null;
		}

		public void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object obj)
		{
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string idPrefix1, string prefix1, string idPrefix2, string prefix2)
		{
		}
	}
}

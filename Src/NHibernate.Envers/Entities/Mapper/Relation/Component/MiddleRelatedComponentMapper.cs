using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	[Serializable]
	public class MiddleRelatedComponentMapper : IMiddleComponentMapper
	{
		private readonly MiddleIdData _relatedIdData;

		public MiddleRelatedComponentMapper(MiddleIdData relatedIdData)
		{
			_relatedIdData = relatedIdData;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator,
										 IDictionary data,
														object dataObject,
										 long revision)
		{
			return entityInstantiator.CreateInstanceFromVersionsEntity(_relatedIdData.EntityName, data, revision);
		}

		public void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object obj)
		{
			_relatedIdData.PrefixedMapper.MapToMapFromEntity(idData, obj);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string idPrefix1, string prefix1, string idPrefix2, string prefix2)
		{
			_relatedIdData.PrefixedMapper.AddIdsEqualToQuery(parameters, idPrefix1, idPrefix2);
		}
	}
}

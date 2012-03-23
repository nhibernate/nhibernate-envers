using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	/// <summary>
	/// A component mapper for the @MapKey mapping: the value of the map's key is the id of the entity. This
	/// doesn't have an effect on the data stored in the versions tables, so <code>mapToMapFromObject</code> is
	/// empty.
	/// </summary>
	public sealed class MiddleMapKeyIdComponentMapper : IMiddleComponentMapper
	{
		private readonly AuditEntitiesConfiguration _verEntCfg;
		private readonly IIdMapper _relatedIdMapper;

		public MiddleMapKeyIdComponentMapper(AuditEntitiesConfiguration verEntCfg, IIdMapper relatedIdMapper)
		{
			_verEntCfg = verEntCfg;
			_relatedIdMapper = relatedIdMapper;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator,
											IDictionary data,
											object dataObject,
											long revision)
		{
			return _relatedIdMapper.MapToIdFromMap((IDictionary)data[_verEntCfg.OriginalIdPropName]);
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

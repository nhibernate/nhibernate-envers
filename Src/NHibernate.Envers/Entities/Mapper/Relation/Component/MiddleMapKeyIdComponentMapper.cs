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
		private readonly AuditEntitiesConfiguration verEntCfg;
		private readonly IIdMapper relatedIdMapper;

		public MiddleMapKeyIdComponentMapper(AuditEntitiesConfiguration verEntCfg, IIdMapper relatedIdMapper)
		{
			this.verEntCfg = verEntCfg;
			this.relatedIdMapper = relatedIdMapper;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator,
											IDictionary data,
											object dataObject,
											long revision)
		{
			return relatedIdMapper.MapToIdFromMap((IDictionary)data[verEntCfg.OriginalIdPropName]);
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

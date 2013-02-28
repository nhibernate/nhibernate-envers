using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
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

		public void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object obj)
		{
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string idPrefix1, string prefix1, string idPrefix2, string prefix2)
		{
		}
	}
}

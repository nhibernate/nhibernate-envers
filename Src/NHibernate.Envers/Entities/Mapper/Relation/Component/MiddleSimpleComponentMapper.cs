using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	[Serializable]
	public class MiddleSimpleComponentMapper : IMiddleComponentMapper
	{
		private readonly AuditEntitiesConfiguration _verEntCfg;
		private readonly string _propertyName;

		public MiddleSimpleComponentMapper(AuditEntitiesConfiguration verEntCfg, string propertyName)
		{
			_propertyName = propertyName;
			_verEntCfg = verEntCfg;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary data, object dataObject, long revision)
		{
			return ((IDictionary)data[_verEntCfg.OriginalIdPropName])[_propertyName];
		}

		public void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object obj)
		{
			idData.Add(_propertyName, obj);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string idPrefix1, string prefix1, string idPrefix2, string prefix2)
		{
			parameters.AddWhere(idPrefix1 + "." + _propertyName, false, "=", idPrefix2 + "." + _propertyName, false);
		}
	}
}

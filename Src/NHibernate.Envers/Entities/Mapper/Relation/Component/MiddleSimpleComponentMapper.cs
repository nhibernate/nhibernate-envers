using System;
using System.Collections;
using System.Collections.Generic;
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

		public void MapToMapFromObject(IDictionary<string, object> data, object obj)
		{
			data.Add(_propertyName, obj);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2)
		{
			parameters.AddWhere(prefix1 + "." + _propertyName, false, "=", prefix2 + "." + _propertyName, false);
		}
	}
}

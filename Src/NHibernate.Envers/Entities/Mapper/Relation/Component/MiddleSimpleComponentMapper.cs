using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	public class MiddleSimpleComponentMapper : IMiddleComponentMapper
	{
		private readonly AuditEntitiesConfiguration verEntCfg;
		private readonly string propertyName;

		public MiddleSimpleComponentMapper(AuditEntitiesConfiguration verEntCfg, string propertyName)
		{
			this.propertyName = propertyName;
			this.verEntCfg = verEntCfg;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary data, object dataObject, long revision)
		{
			return ((IDictionary)data[verEntCfg.OriginalIdPropName])[propertyName];
		}

		public void MapToMapFromObject(IDictionary<string, object> data, object obj)
		{
			data.Add(propertyName, obj);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2)
		{
			parameters.AddWhere(prefix1 + "." + propertyName, false, "=", prefix2 + "." + propertyName, false);
		}
	}
}

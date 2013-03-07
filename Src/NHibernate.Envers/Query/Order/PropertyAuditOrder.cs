using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query.Order
{
	public class PropertyAuditOrder : IAuditOrder
	{
		private readonly IPropertyNameGetter _propertyNameGetter;
		private readonly bool _asc;

		public PropertyAuditOrder(IPropertyNameGetter propertyNameGetter, bool asc)
		{
			_propertyNameGetter = propertyNameGetter;
			_asc = asc;
		}

		public Tuple<string, bool> GetData(AuditConfiguration auditCfg)
		{
			return new Tuple<String, Boolean>(_propertyNameGetter.Get(auditCfg), _asc);
		}
	}
}

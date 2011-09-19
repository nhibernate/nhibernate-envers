using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Order
{
	public class PropertyAuditOrder : IAuditOrder
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly bool asc;

		public PropertyAuditOrder(IPropertyNameGetter propertyNameGetter, bool asc)
		{
			this.propertyNameGetter = propertyNameGetter;
			this.asc = asc;
		}

		public Pair<String, Boolean> GetData(AuditConfiguration auditCfg)
		{
			return new Pair<String, Boolean>(propertyNameGetter.Get(auditCfg), asc);
		}
	}
}

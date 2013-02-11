using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query.Projection
{
	public class PropertyAuditProjection : IAuditProjection
	{
		private readonly IPropertyNameGetter _propertyNameGetter;
		private readonly string _function;
		private readonly bool _distinct;

		public PropertyAuditProjection(IPropertyNameGetter propertyNameGetter, string function, bool distinct)
		{
			_propertyNameGetter = propertyNameGetter;
			_function = function;
			_distinct = distinct;
		}

		public Tuple<string, string, bool> GetData(AuditConfiguration auditCfg)
		{
			var propertyName = _propertyNameGetter.Get(auditCfg);

			return new Tuple<string, string, bool>(_function, propertyName, _distinct);
		}
	}
}

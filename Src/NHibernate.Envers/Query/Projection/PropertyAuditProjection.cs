using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Projection
{
	public class PropertyAuditProjection : IAuditProjection
	{
		private readonly IPropertyNameGetter propertyNameGetter;
		private readonly string function;
		private readonly bool distinct;

		public PropertyAuditProjection(IPropertyNameGetter propertyNameGetter, string function, bool distinct)
		{
			this.propertyNameGetter = propertyNameGetter;
			this.function = function;
			this.distinct = distinct;
		}

		public Triple<string, string, bool> GetData(AuditConfiguration auditCfg)
		{
			var propertyName = propertyNameGetter.Get(auditCfg);

			return new Triple<string, string, bool>(function, propertyName, distinct);
		}
	}
}

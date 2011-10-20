using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AuditTableAttribute : Attribute
	{
		public AuditTableAttribute(string value)
		{
			Value = value;
			Schema = string.Empty;
			Catalog = string.Empty;
		}

		public string Value { get; set; }
		public string Schema { get; set; }
		public string Catalog { get; set; }
	}
}

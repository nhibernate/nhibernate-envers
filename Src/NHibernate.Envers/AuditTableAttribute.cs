using System;

namespace NHibernate.Envers
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AuditTableAttribute : Attribute
	{
		public AuditTableAttribute(string value)
		{
			Value = value;
			Schema = string.Empty;
			Catalog = string.Empty;
		}

		public string Value { get; private set; }
		public string Schema { get; set; }
		public string Catalog { get; set; }
	}
}

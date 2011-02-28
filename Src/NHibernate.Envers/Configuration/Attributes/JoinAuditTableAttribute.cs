using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	//In (Java) Envers - called "Secondary" instead of Join
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class JoinAuditTableAttribute : Attribute
	{
		public string JoinTableName { get; set; }
		public string JoinAuditTableName { get; set; }
	}
}

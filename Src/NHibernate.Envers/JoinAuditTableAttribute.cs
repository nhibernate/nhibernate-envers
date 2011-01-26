using System;

namespace NHibernate.Envers
{
	//In (Java) Envers - called "Secondary" instead of Join
	[AttributeUsage(AttributeTargets.Class)]
	public class JoinAuditTableAttribute : Attribute
	{
		public string JoinTableName { get; set; }
		public string JoinAuditTableName { get; set; }
	}
}

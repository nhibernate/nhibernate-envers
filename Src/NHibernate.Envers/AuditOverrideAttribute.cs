using System;

namespace NHibernate.Envers
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class AuditOverrideAttribute:Attribute
	{
		public AuditOverrideAttribute()
		{
			IsAudited = true;
			AuditJoinTable = new AuditJoinTableAttribute();
		}

		/**
		 * @return <strong>Required</strong> Name of the field (or property) whose mapping
		 * is being overridden.
		 */
		public string Name { get; set; }

		/**
		 * @return Indicates if the field (or property) is audited; defaults to {@code true}.
		 */
		public bool IsAudited { get; set; }

		/**
		 * @return New {@link AuditJoinTable} used for this field (or property). Its value
		 * is ignored if {@link #isAudited()} equals to {@code false}.
		 */
		public AuditJoinTableAttribute AuditJoinTable { get; set; }
	}
}

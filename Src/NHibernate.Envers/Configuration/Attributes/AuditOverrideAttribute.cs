using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class AuditOverrideAttribute : Attribute
	{
		public AuditOverrideAttribute()
		{
			IsAudited = true;
			AuditJoinTable = new AuditJoinTableAttribute();
		}

		/// <summary>
		///  <strong>Required</strong> Name of the field (or property) whose mapping
		///  is being overridden.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Indicates if the field (or property) is audited; defaults to {@code true}.
		/// </summary>
		public bool IsAudited { get; set; }

		/// <summary>
		/// @return New {@link AuditJoinTable} used for this field (or property). Its value
		/// is ignored if {@link #isAudited()} equals to {@code false}.
		/// </summary>
		public AuditJoinTableAttribute AuditJoinTable { get; set; }
	}
}

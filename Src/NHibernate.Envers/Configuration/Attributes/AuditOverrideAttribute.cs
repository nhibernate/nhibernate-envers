using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// The AuditingOverrideAttribute is used to override the auditing
	/// behavior of a superclass or single property inherited from 
	/// a non mapped base type, or inside an embedded component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
	public sealed class AuditOverrideAttribute : AuditJoinTableAttribute
	{
		public AuditOverrideAttribute()
		{
			IsAudited = true;
		}

		/// <summary>
		/// Name of the field (or property) whose mapping is being overridden.
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// Indicates if the field (or property) is audited; defaults to <code>true</code>.
		/// </summary>
		public bool IsAudited { get; set; }

		/// <summary>
		/// Can be used together with <seealso cref="IsAudited"/> to set all properties to be audited or not.
		/// </summary>
		public System.Type ForClass { get; set; }
	}
}

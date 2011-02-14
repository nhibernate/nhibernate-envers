using System;

namespace NHibernate.Envers
{
	/// <summary>
	/// When applied to a field, indicates that this field should not be audited.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class NotAuditedAttribute : Attribute
	{
	}
}

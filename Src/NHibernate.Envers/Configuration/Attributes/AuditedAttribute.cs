using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// When applied to a class, indicates that all of its properties should be audited.
	/// When applied to a field, indicates that this field should be audited.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property |  AttributeTargets.Field | AttributeTargets.Interface)]
	public sealed class AuditedAttribute : Attribute
	{
		public AuditedAttribute()
		{
			TargetAuditMode = RelationTargetAuditMode.Audited;
		}

		/// <summary>
		/// Specifies if the entity that is the target of the relation should be audited or not. If not, then when
		/// reading a historic version an audited entity, the relation will always point to the "current" entity.
		/// This is useful for dictionary-like entities, which don't change and don't need to be audited.
		/// </summary>
		public RelationTargetAuditMode TargetAuditMode { get; set; }

		/// <summary>
		/// Should a modification flag be stored for each property in the annotated class or for the annotated
		/// property. The flag stores information if a property has been changed at a given revision.
		/// This can be used for example in queries.
		/// </summary>
		public bool WithModifiedFlag { get; set; }
	}
}

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
			ModStore = ModificationStore.Full;
			AuditParents = new System.Type[0];
		}

		public ModificationStore ModStore { get; private set; }

		/// <summary>
		/// Specifies if the entity that is the target of the relation should be audited or not. If not, then when
		/// reading a historic version an audited entity, the relation will always point to the "current" entity.
		/// This is useful for dictionary-like entities, which don't change and don't need to be audited.
		/// </summary>
		public RelationTargetAuditMode TargetAuditMode { get; set; }

		/// <summary>
		/// Set of superclasses which properties shall be audited. The behavior of listed classes
		/// is the same as if they had <see cref="AuditedAttribute"/> annotation applied on a type level. The scope of this functionality
		/// is limited to the context of actually mapped entity and its class hierarchy. If a parent type lists any of
		/// its parent types using this attribute, all fields encapsulated by marked classes are implicitly audited.
		/// </summary>
		public System.Type[] AuditParents { get; set; }
	}
}

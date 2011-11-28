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
		/// Specifies the superclasses for which properties should be audited, even if the superclasses are not
		/// annotated with <see cref="AuditedAttribute"/>. Causes all properties of the listed classes to be audited, just as if the
		/// classes had <see cref="AuditedAttribute"/> annotation applied on the class level.
		/// 
		/// The scope of this functionality is limited to the class hierarchy of the annotated entity.
		/// 
		/// If a parent type lists any of its parent types using this attribute, all properties in the specified classes
		/// will also be audited.
		/// </summary>
		public System.Type[] AuditParents { get; set; }
	}
}

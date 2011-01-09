using System;

namespace NHibernate.Envers
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property |  AttributeTargets.Field)]
	public class AuditedAttribute:Attribute
	{
		public ModificationStore ModStore = ModificationStore.FULL;

		/**
		 * @return Specifies if the entity that is the target of the relation should be audited or not. If not, then when
		 * reading a historic version an audited entity, the realtion will always point to the "current" entity.
		 * This is useful for dictionary-like entities, which don't change and don't need to be audited.
		 */
		public RelationTargetAuditMode TargetAuditMode =  RelationTargetAuditMode.AUDITED;
	}
}

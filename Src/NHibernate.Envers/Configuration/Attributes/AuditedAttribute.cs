﻿using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property |  AttributeTargets.Field)]
	public sealed class AuditedAttribute : Attribute
	{
		public AuditedAttribute()
		{
			TargetAuditMode = RelationTargetAuditMode.Audited;
			ModStore = ModificationStore.Full;
		}

		public ModificationStore ModStore { get; private set; }

		/// <summary>
		/// @return Specifies if the entity that is the target of the relation should be audited or not. If not, then when
		/// reading a historic version an audited entity, the realtion will always point to the "current" entity.
		/// This is useful for dictionary-like entities, which don't change and don't need to be audited.
		/// </summary>
		public RelationTargetAuditMode TargetAuditMode { get; set; }
	}
}

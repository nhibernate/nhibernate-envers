using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	/// <summary>
	/// Revision entity which <see cref="ModifiedEntityTypes"/> property is manually populated by <see cref="CustomTrackingRevisionListener"/>.
	/// </summary>
	[RevisionEntity(typeof(CustomTrackingRevisionListener))]
	public class CustomTrackingRevisionEntity 
	{
		public CustomTrackingRevisionEntity()
		{
			ModifiedEntityTypes = new HashSet<ModifiedEntityTypeEntity>();
		}

		[RevisionNumber]
		public virtual int CustomId { get; set; }

		[RevisionTimestamp]
		public virtual long CustomTimestamp { get; set; }

		public virtual ISet<ModifiedEntityTypeEntity> ModifiedEntityTypes { get; set; }

		public virtual void AddModifiedEntityType(string entityName)
		{
			ModifiedEntityTypes.Add(new ModifiedEntityTypeEntity {Revision = this, EntityName = entityName});
		}

		public override bool Equals(object obj)
		{
			var casted = obj as CustomTrackingRevisionEntity;
			if (casted == null)
				return false;
			return CustomId == casted.CustomId &&
					 CustomTimestamp == casted.CustomTimestamp;
		}

		public override int GetHashCode()
		{
			return CustomId ^ CustomTimestamp.GetHashCode();
		}
	}
}
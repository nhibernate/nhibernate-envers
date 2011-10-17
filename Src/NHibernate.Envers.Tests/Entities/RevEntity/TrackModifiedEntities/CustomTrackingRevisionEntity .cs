using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	[RevisionEntity(typeof(CustomEntityTrackingRevisionListener))]
	public class CustomTrackingRevisionEntity 
	{
		public CustomTrackingRevisionEntity()
		{
			ModifiedEntityNames = new HashedSet<ModifiedEntityNameEntity>();
		}

		[RevisionNumber]
		public virtual int CustomId { get; set; }

		[RevisionTimestamp]
		public virtual long CustomTimestamp { get; set; }

		public virtual ISet<ModifiedEntityNameEntity> ModifiedEntityNames { get; set; }

		public virtual void AddModifiedEntityName(string entityName)
		{
			ModifiedEntityNames.Add(new ModifiedEntityNameEntity {Revision = this, EntityName = entityName});
		}

		public virtual void RemoveModifiedEntityName(string entityName)
		{
			ModifiedEntityNames.Remove(new ModifiedEntityNameEntity {Revision = this, EntityName = entityName});
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
using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	[RevisionEntity]
	public class AnnotatedTrackingRevisionEntity
	{
		public AnnotatedTrackingRevisionEntity()
		{
			ModifiedEntityNames = new HashedSet<string>();
		}

		[RevisionNumber]
		public virtual int CustomId { get; set; }

		[RevisionTimestamp]
		public virtual long CustomTimestamp { get; set; }

		[ModifiedEntityNames]
		public virtual ISet<string> ModifiedEntityNames { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as AnnotatedTrackingRevisionEntity;
			if (casted == null)
				return false;
			return CustomId == casted.CustomId &&
			       CustomTimestamp == casted.CustomTimestamp &&
			       ModifiedEntityNames.Equals(casted.ModifiedEntityNames);
		}

		public override int GetHashCode()
		{
			return CustomId ^ CustomTimestamp.GetHashCode() ^ ModifiedEntityNames.GetHashCode();
		}
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	/// <summary>
	/// Sample revision entity that uses <see cref="ModifiedEntityNamesAttribute"/> annotation.
	/// </summary>
	[RevisionEntity]
	public class AnnotatedTrackingRevisionEntity
	{
		[RevisionNumber]
		public virtual int CustomId { get; set; }

		[RevisionTimestamp]
		public virtual long CustomTimestamp { get; set; }

		[ModifiedEntityNames]
		public virtual ISet<string> EntityNames { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as AnnotatedTrackingRevisionEntity;
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
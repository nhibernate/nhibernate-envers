using System;

namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	[RevisionEntity]
	public class CustomDateRevEntity
	{
		[RevisionNumber]
		public virtual long CustomId { get; set; }

		[RevisionTimestamp]
		public virtual DateTime DateTimestamp { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as CustomDateRevEntity;
			if (casted == null)
				return false;
			return (CustomId == casted.CustomId && DateTimestamp.Equals(casted.DateTimestamp));
		}

		public override int GetHashCode()
		{
			return CustomId.GetHashCode() ^ DateTimestamp.GetHashCode();
		}
	}
}
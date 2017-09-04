using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers
{
	[Serializable]
	public class DefaultRevisionEntity
	{
		[RevisionNumber]
		public virtual int Id { get; set; }

		[RevisionTimestamp]
		public virtual DateTime RevisionDate { get; set; }

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			if (!(obj is DefaultRevisionEntity revisionEntity)) return false;

			var that = revisionEntity;

			if (Id != that.Id) return false;
			return RevisionDate == that.RevisionDate;
		}

		public override int GetHashCode()
		{
			var result = Id;
			result = 31 * result + (int)(((ulong)RevisionDate.Ticks) ^ (((ulong)RevisionDate.Ticks) >> 32));
			return result;
		}
	}
}

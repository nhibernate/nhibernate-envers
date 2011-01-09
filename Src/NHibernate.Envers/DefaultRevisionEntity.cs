using System;

namespace NHibernate.Envers
{
	[Serializable]
	public class DefaultRevisionEntity 
	{
		[RevisionNumber]
		public virtual int Id {get; set;}

		[RevisionTimestamp]
		public virtual DateTime RevisionDate { get; set; }

		public override bool Equals(object o) 
		{
			if (this == o) return true;
			var revisionEntity = o as DefaultRevisionEntity;
			if (revisionEntity == null) return false;

			var that = revisionEntity;

			if (Id != that.Id) return false;
			return RevisionDate == that.RevisionDate;
		}

		public override int GetHashCode() 
		{
			var result = Id;
			result = 31 * result + (int) (((ulong)RevisionDate.Ticks) ^ (((ulong)RevisionDate.Ticks) >> 32));
			return result;
		}

		public override string ToString() 
		{
			return "DefaultRevisionEntity(id = " + Id + ", revisionDate = " + RevisionDate.ToShortDateString() + ")";
		}
	}
}

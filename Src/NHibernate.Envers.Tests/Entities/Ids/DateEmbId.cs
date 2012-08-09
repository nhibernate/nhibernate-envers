using System;

namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class DateEmbId
	{
		public virtual DateTime X { get; set; }
		public virtual DateTime Y { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as DateEmbId;
			if (casted == null)
				return false;
			return casted.X == X && casted.Y == Y;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}
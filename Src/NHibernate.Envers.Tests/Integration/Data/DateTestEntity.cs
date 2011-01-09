using System;

namespace NHibernate.Envers.Tests.Integration.Data
{
	public class DateTestEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual DateTime Date { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as DateTestEntity;
			if (other == null)
				return false;
			return other.Id == Id && other.Date == Date;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Date.GetHashCode();
		}
	}
}
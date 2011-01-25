namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	[RevisionEntity(typeof(TestExceptionRevisionListener))]
	public class ExceptionListenerRevEntity
	{
		[RevisionNumber]
		public virtual int Id { get; set; }

		[RevisionTimestamp]
		public virtual long Timestamp { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ExceptionListenerRevEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Timestamp == casted.Timestamp);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Timestamp.GetHashCode();
		}
	}
}
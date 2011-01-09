namespace NHibernate.Envers.Tests.Entities
{
	public class StrTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Str { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as StrTestEntity;
			if (other == null)
				return false;
			return Id == other.Id && Str.Equals(other.Str);			
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Str.GetHashCode();
		}
	}
}
namespace NHibernate.Envers.Tests.Entities
{
	public class StrIntTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Str { get; set; }
		[Audited]
		public virtual int Number { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as StrIntTestEntity;
			if (other == null)
				return false;
			return Id == other.Id && Str.Equals(other.Str) && Number==other.Number;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Str.GetHashCode() ^Number;
		}
	}
}
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
			return Id == other.Id && strEquals(other) && Number == other.Number;
		}

		private bool strEquals(StrIntTestEntity other)
		{
			if (other.Str == null && Str == null)
				return true;
			if (other.Str == null || Str == null)
				return false;
			return Str.Equals(other.Str);
		}

		public override int GetHashCode()
		{
			var strValue = Str == null ? 0 : Str.GetHashCode();
			return Id.GetHashCode() ^ strValue ^Number;
		}
	}
}
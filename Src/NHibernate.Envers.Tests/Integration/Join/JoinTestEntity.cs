namespace NHibernate.Envers.Tests.Integration.Join
{
	[Audited]
	public class JoinTestEntity
	{
		public virtual int Id { get; set; }
		public virtual string S1 { get; set; }
		public virtual string S2 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as JoinTestEntity;
			if (other == null)
				return false;
			return Id == other.Id && S1.Equals(other.S1) && S2.Equals(other.S2);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ S1.GetHashCode() ^ S2.GetHashCode();
		}
	}
}
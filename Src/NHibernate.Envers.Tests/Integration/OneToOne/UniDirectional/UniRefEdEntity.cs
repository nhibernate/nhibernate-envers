namespace NHibernate.Envers.Tests.Integration.OneToOne.UniDirectional
{
	public class UniRefEdEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as UniRefEdEntity;
			if (other == null)
				return false;
			return Id == other.Id && Data.Equals(other.Data);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Data.GetHashCode();
		}
	}
}
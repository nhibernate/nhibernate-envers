namespace NHibernate.Envers.Tests.Integration.Naming
{
	public class JoinNamingRefIngEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }
		[Audited]
		public virtual JoinNamingRefEdEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as JoinNamingRefIngEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}
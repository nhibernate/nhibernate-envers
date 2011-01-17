namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	public class BiRefIngEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual BiRefEdEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as BiRefIngEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data == casted.Data);
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}
namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	public class BiRefEdEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual BiRefIngEntity Referencing { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as BiRefEdEntity;
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
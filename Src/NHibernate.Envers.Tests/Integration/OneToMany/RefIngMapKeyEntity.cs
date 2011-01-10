namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	public class RefIngMapKeyEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual RefEdMapKeyEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as RefIngMapKeyEntity;
			if (casted == null)
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}
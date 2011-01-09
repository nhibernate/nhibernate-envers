namespace NHibernate.Envers.Tests.Entities.ManyToMany
{
	public class IntNoAutoIdTestEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual int Number { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as IntNoAutoIdTestEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Number == casted.Number);
		}

		public override int GetHashCode()
		{
			return Id ^ Number.GetHashCode();
		}
	}
}
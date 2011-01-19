namespace NHibernate.Envers.Tests.Integration.Primitive
{
	public class PrimitiveTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual int Number { get; set; }
		public virtual int Number2 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as PrimitiveTestEntity;
			if (other == null)
				return false;
			return Id == other.Id && Number == other.Number && Number2 == other.Number2;
		}

		public override int GetHashCode()
		{
			return Id ^ Number ^ Number2;
		}
	}
}
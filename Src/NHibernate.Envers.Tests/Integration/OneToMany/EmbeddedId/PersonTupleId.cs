namespace NHibernate.Envers.Tests.Integration.OneToMany.EmbeddedId
{
	public class PersonTupleId
	{
		public long PersonAId { get; set; }
		public long PersonBId { get; set; }
		public string ConstantId { get; set; }
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PersonTupleId)) return false;
			return Equals((PersonTupleId)obj);
		}

		public bool Equals(PersonTupleId other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.PersonAId == PersonAId && other.PersonBId == PersonBId && Equals(other.ConstantId, ConstantId);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = PersonAId.GetHashCode();
				result = (result * 397) ^ PersonBId.GetHashCode();
				result = (result * 397) ^ (ConstantId != null ? ConstantId.GetHashCode() : 0);
				return result;
			}
		}
	}
}
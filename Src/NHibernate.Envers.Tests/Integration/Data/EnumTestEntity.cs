namespace NHibernate.Envers.Tests.Integration.Data
{
	public enum E1 { X, Y }
	public enum E2 { A, B }

	[Audited]
	public class EnumTestEntity
	{
		public virtual int Id { get; set; }
		public virtual E1 Enum1 { get; set; }
		public virtual E2 Enum2 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as EnumTestEntity;
			if (other == null)
				return false;
			return other.Id == Id && other.Enum1 == Enum1 && other.Enum2 == Enum2;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Enum1.GetHashCode() ^ Enum2.GetHashCode();
		}
	}
}
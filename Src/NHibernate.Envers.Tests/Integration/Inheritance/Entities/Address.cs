namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class Address
	{
		public virtual long Id { get; set; }
		public virtual string Address1 { get; set; }
		public virtual Contact Contact { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as Address;
			if (casted == null)
				return false;
			return Id == casted.Id && Address1.Equals(casted.Address1);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Address1.GetHashCode();
		}
	}
}
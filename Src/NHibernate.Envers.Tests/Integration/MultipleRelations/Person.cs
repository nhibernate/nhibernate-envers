using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.MultipleRelations
{
	[Audited]
	public class Person
	{
		public Person()
		{
			Addresses = new HashSet<Address>();
			OwnedAddresses = new HashSet<Address>();
		}

		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Address> Addresses { get; set; }
		public virtual ISet<Address> OwnedAddresses { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as Person;
			if (casted == null)
				return false;
			if (Name != null ? !Name.Equals(casted.Name) : casted.Name != null)
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			var res = Id.GetHashCode();
			return 31 * res + (Name != null ? Name.GetHashCode() : 0);
		}
	}
}
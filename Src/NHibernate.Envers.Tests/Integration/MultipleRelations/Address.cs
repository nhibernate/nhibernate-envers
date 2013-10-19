using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.MultipleRelations
{
	[Audited]
	public class Address
	{
		public Address()
		{
			Tenants = new HashSet<Person>();
		}

		public virtual long Id { get; set; }
		public virtual string City { get; set; }
		public virtual ISet<Person> Tenants { get; set; }
		public virtual Person Landlord { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as Address;
			if (casted == null)
				return false;
			if (City != null ? !City.Equals(casted.City) : casted.City != null) 
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			var res = Id.GetHashCode();
			return 31 * res + (City != null ? City.GetHashCode() : 0);
		}
	}
}
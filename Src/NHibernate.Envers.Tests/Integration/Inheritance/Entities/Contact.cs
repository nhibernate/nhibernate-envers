using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class Contact
	{
		public virtual long Id { get; set; }
		public virtual string Email { get; set; }
		public virtual ISet<Address> Addresses { get; set; }
	}
}
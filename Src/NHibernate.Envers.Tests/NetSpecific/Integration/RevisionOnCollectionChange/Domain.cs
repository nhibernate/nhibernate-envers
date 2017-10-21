using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.RevisionOnCollectionChange
{
	[Audited]
	public class Person
	{
		public Person()
		{
			Cars = new List<Car>();
		}
		public virtual Guid Id { get; set; }

		public virtual IList<Car> Cars { get; protected set; }
	}

	[Audited]
	public class Car
	{
		public virtual Guid Id { get; set; }

		public virtual Person Owner { get; set; }
	}
}
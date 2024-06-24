using NHibernate.Envers.Configuration.Attributes;
using System.Collections.Generic;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ManyToOne.LazyProperty.Bidirectional
{
	[Audited]
	public class Person
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Car> Cars { get; set; }
	}
}
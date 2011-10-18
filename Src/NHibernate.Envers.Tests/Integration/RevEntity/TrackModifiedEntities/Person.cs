using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	[Audited]
	public class Person
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Age { get; set; }
		public virtual IList<Car> Cars { get; set; }
	}
}
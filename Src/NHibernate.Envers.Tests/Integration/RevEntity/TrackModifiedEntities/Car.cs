using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	[Audited]
	public class Car
	{
		public virtual long Id { get; set; }
		public virtual int Number { get; set; }
		public virtual IList<Person> Owners { get; set; }
	}
}
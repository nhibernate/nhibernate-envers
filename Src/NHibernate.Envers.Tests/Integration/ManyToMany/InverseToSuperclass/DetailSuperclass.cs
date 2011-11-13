using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.InverseToSuperclass
{
	[Audited]
	public class DetailSuperclass
	{
		public virtual long Id { get; set; }
		public virtual IList<Master> Masters { get; set; }
	}
}
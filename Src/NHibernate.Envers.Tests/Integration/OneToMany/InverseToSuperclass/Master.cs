using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.OneToMany.InverseToSuperclass
{
	[Audited]
	public class Master
	{
		public virtual long Id { get; set; }
		public virtual string Str { get; set; }
		public virtual IList<DetailSubclass> Items { get; set; }
	}
}
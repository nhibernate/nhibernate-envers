using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Entities
{
	[Audited]
	public class ListParent
	{
		public virtual Guid Id { get; set; }
		public virtual IList<ListChild> Children { get; set; }
	}

	[Audited]
	public class ListChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
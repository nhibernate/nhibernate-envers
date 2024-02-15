using NHibernate.Envers.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Lazy
{
	[Audited]
	public class EntityWithLazyProp
	{
		public virtual int Id { get; set; }
		public virtual string NotLazyProp { get; set; }
		public virtual string LazyProp { get; set; }
	}
}

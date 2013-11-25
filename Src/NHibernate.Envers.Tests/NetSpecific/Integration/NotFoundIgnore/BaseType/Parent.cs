using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.NotFoundIgnore.BaseType
{
	[Audited]
	public class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual IList<Child> Children { get; set; }
	}
}
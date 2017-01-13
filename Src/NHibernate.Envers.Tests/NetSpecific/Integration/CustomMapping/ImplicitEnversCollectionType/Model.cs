using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.ImplicitEnversCollectionType
{
	[Audited]
	public class Parent
	{
		public virtual int Id { get; set; }
		public virtual IList<Child> Children { get; set; }
	}

	[Audited]
	public class Child
	{
		public virtual int Id { get; set; }
	}
}
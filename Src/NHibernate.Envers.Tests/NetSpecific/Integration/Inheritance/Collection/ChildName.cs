using System;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Inheritance.Collection
{
	public class ChildName
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
using System;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.NotFoundIgnore.BaseType
{
	public class ChildName
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
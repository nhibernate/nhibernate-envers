using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Incorrect
{
	public class BaseEntity
	{
		public virtual int Id { get; set; }
	}

	[Audited]
	public class ConcreteEntity
	{
		public virtual int Data { get; set; }
	}
}
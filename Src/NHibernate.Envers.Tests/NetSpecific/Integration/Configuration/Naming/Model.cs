using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Naming
{
	[Audited]
	public abstract class Base
	{
		public virtual int Id { get; set; }
	}

	[Audited]
	public class Concrete : Base
	{
		public virtual int Data { get; set; }
	}
}
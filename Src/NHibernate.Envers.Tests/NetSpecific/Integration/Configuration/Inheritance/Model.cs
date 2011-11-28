using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Inheritance
{
	[Audited]
	public class Super : Base
	{
		public virtual string Data { get; set; }
	}

	[Audited]
	public class Base
	{
		public virtual int Id { get; set; }

		[NotAudited]
		public virtual int Number { get; set; }
	}
}
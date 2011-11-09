using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration
{
	[Audited]
	public class SimpleAuiditableForConfEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
	}
}
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Component
{
	[Audited]
	public class BaseClassOwner
	{
		public virtual int Id { get; set; }
		public virtual BaseClassComponent Component { get; set; }
	}
}

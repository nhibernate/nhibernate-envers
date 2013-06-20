using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Component
{
	[Audited]
	public class DerivedClassOwner
	{
		public virtual int Id { get; set; }
		public virtual DerivedClassComponent Component { get; set; }
	}
}

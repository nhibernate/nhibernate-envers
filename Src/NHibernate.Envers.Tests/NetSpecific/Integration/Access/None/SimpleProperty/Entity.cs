using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Access.None.SimpleProperty
{
	[Audited]
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual int Data { get; set; }
	}
}
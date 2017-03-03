using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.MultiLevelInheritance
{
	[Audited]
	public class Child
	{
		public virtual int Id { get; set; }
		public virtual Parent Parent { get; set; }
	}
}
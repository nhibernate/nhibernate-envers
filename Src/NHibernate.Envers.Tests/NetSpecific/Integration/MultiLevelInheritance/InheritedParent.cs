using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.MultiLevelInheritance
{
	[Audited]
	public class InheritedParent : Parent
	{
		public virtual string InheritedProperty { get; set; }
	}
}

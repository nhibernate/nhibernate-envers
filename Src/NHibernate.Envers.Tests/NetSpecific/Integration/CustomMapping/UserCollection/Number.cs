using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	[Audited]
	public class Number
	{
		public virtual int Id { get; set; }
		public virtual int Value { get; set; }
	}
}
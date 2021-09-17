using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.LazyProperty
{
	[Audited]
	public class Person
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
	}
}
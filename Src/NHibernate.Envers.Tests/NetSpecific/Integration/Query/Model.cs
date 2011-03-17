using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Query
{
	[Audited]
	public class Person
	{
		public virtual int Id { get; set; }
		public virtual Weight Weight { get; set; }
	}

	public class Weight
	{
		public virtual int Kilo { get; set; }
	}
}
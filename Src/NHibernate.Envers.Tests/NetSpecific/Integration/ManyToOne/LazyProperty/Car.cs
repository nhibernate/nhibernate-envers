using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ManyToOne.LazyProperty
{
	[Audited]
	public class Car
	{
		public virtual long Id { get; set; }
		public virtual int Number { get; set; }
		public virtual Person Owner { get; set; }
	}
}
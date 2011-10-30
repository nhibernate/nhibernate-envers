using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.EntityNames.SingleAssociatedAudited
{
	[Audited]
	public class Car
	{
		public virtual long Id { get; set; }
		public virtual int Number { get; set; }
		public virtual Person Owner { get; set; }
		public virtual Person Driver { get; set; }
	}
}
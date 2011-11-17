using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Mixed
{
	[Audited]
	public abstract class AbstractActivity : IActivity
	{
		public virtual ActivityId Id { get; set; }
		public virtual int SequenceNumber { get; set; }
	}
}
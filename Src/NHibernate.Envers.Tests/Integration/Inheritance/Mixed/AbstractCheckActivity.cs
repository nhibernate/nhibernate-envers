using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Mixed
{
	[Audited]
	public abstract class AbstractCheckActivity : AbstractActivity
	{
		public virtual int DurationInMinutes { get; set; }
		public virtual IActivity RelatedActivity { get; set; }
	}
}
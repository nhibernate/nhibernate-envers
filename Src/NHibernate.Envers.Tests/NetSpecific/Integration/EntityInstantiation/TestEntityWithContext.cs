using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.EntityInstantiation
{
	[Audited]
	public class TestEntityWithContext
	{
		public virtual int Id { get; protected set; }
		public virtual string StringValue { get; set; }
		public virtual bool CreatedByFactory { get; protected set; }
		public virtual IExternalContext Context { get; set; }

		public TestEntityWithContext()
		{
		}

		public TestEntityWithContext(bool createdByFactory)
		{
			CreatedByFactory = createdByFactory;
		}
	}
}

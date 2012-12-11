using NHibernate.Envers.Event;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.EntityInstantiation
{
	public class PostInstantiationListener : IPostInstantiationListener
	{
		public void PostInstantiate(object entity)
		{
			var testEnt = (TestEntityWithContext)entity;
			testEnt.Context = new TestExternalContext();
		}
	}
}

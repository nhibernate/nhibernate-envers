namespace NHibernate.Envers.Tests.NetSpecific.Integration.EntityInstantiation
{
	public class TestExternalContext : IExternalContext
	{
		public string ContextName { get { return "Test Context"; } }
	}
}

namespace NHibernate.Envers.Tests.Integration.EntityInstantiation
{
	public class TestExternalContext : IExternalContext
	{
		public string ContextName { get { return "Test Context"; } }
	}
}

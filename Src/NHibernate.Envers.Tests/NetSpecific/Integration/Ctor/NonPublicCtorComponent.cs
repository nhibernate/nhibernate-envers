namespace NHibernate.Envers.Tests.NetSpecific.Integration.Ctor
{
	public class NonPublicCtorComponent
	{
		protected NonPublicCtorComponent() { }

		public NonPublicCtorComponent(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}
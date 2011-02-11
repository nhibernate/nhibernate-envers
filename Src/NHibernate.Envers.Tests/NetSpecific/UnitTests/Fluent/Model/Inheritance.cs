namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model
{
	public class Animal
	{
		private int weight;
		public string Name { get; set; }
	}

	public class Dog : Animal
	{
	}

	public class Cat : Animal{}
}
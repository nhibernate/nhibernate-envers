namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model
{
	public class Animal
	{
#pragma warning disable 0169
		private int weight;
#pragma warning restore 0169

		public string Name { get; set; }
	}

	public class Dog : Animal
	{
	}

	public class Cat : Animal { }
}
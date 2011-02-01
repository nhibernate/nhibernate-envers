namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model
{
    public abstract class Animal
    {
        private int weight;
    }

    public class Dog : Animal
    {
    }

    public class Cat : Animal{}
}
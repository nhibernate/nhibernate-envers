namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model
{
	public class PublicFieldAndMethodEntity
	{
		public int SomeField;
		public int SomeMethod()
		{
			return 0;
		}
	}
}
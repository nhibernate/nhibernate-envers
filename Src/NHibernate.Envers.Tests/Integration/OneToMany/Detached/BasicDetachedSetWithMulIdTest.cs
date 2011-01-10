using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	[TestFixture, Ignore("Multiple id is not supported in NH Core")]
	public class BasicDetachedSetWithMulIdTest
	{
		[Test]
		public void VerifyRevisionCount()
		{
		}

		[Test]
		public void VerifyHistoryOfColl1()
		{
		}
	}
}
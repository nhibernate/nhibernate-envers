using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class MethodTest
	{
		[Test]
		public void ExcludingMethodShouldThrow()
		{
			var cfg = new FluentConfiguration();
			Assert.Throws<FluentException>(() =>
				cfg.Audit<MethodEntity>()
					.Exclude(obj => obj.SomeMethod()));
		} 
	}
}
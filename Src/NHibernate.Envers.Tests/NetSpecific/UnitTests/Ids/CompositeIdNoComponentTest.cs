using NHibernate.Cfg;
using NHibernate.Envers.Exceptions;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Ids
{
	[TestFixture]
	public class CompositeIdNoComponentTest
	{
		[Test]
		public void ShouldThrowCorrectException()
		{
			var cfg = new Cfg.Configuration();
			cfg.Configure();
			cfg.AddResource("NHibernate.Envers.Tests.NetSpecific.UnitTests.Ids.Mapping.hbm.xml", GetType().Assembly);

			Assert.Throws<AuditException>(() => cfg.IntegrateWithEnvers());
		}
	}
}
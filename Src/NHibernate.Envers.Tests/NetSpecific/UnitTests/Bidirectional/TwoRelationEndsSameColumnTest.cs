using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Bidirectional
{
	public class TwoRelationEndsSameColumnTest
	{
		[Test, Ignore("NHE-158")]
		public void ShouldNotThrowMappingException()
		{
			var cfg = new Cfg.Configuration();
			cfg.Configure();
			cfg.AddResource("NHibernate.Envers.Tests.NetSpecific.UnitTests.Bidirectional.Mapping.hbm.xml", GetType().Assembly);

			Assert.DoesNotThrow(() => cfg.IntegrateWithEnvers());
		}
	}
}

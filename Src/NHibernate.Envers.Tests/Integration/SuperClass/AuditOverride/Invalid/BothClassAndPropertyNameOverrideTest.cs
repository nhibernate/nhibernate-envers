using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride.Invalid
{
	public class BothClassAndPropertyNameOverrideTest
	{
		[Test]
		public void ShouldResultInMappingException()
		{
			var nhCfg = new Cfg.Configuration();
			nhCfg.Configure()
				.AddResource("NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride.Invalid.Mapping.hbm.xml", GetType().Assembly);

			Assert.Throws<MappingException>(() =>
			                                nhCfg.IntegrateWithEnvers());
		}
	}
}
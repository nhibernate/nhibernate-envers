using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Incorrect
{
	[TestFixture]
	public class NotAuditingBaseClassTest
	{
		[Test]
		public void ShouldThrowMappingException()
		{
			var assembly = GetType().Assembly;
			var assemblyName = assembly.GetName().Name;
			var configuration = new Cfg.Configuration()
							.Configure()
							.AddResource(assemblyName + ".NetSpecific.Integration.Configuration.Incorrect.Mapping.hbm.xml", assembly);
			Assert.Throws<MappingException>(() => configuration.IntegrateWithEnvers());
		}
	}
}
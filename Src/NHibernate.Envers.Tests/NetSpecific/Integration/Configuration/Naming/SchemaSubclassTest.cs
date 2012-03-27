using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Naming
{
	[TestFixture]
	public class SchemaSubclassTest
	{
		private const string enversSchema = "TheSchema";
		private const string enversCatalog = "TheSchema";
		private Cfg.Configuration configuration;

		[SetUp]
		public void Setup()
		{
			var assembly = GetType().Assembly;
			var assemblyName = assembly.GetName().Name;
			configuration = new Cfg.Configuration()
							.SetEnversProperty(ConfigurationKey.DefaultSchema, enversSchema)
							.SetEnversProperty(ConfigurationKey.DefaultCatalog, enversCatalog)
							.Configure()
							.AddResource(assemblyName + ".NetSpecific.Integration.Configuration.Naming.Mapping.hbm.xml", assembly)
							.IntegrateWithEnvers();
		}

		[Test]
		public void ShouldBeOnCorrectSchema()
		{
			var table = configuration.GetClassMapping(typeof(Concrete).FullName + "_AUD").Table;
			table.Schema.Should().Be.EqualTo(enversSchema);
		}

		[Test]
		public void ShouldBeOnCorrectCatalog()
		{
			var table = configuration.GetClassMapping(typeof(Concrete).FullName + "_AUD").Table;
			table.Catalog.Should().Be.EqualTo(enversCatalog);
		}
	}
}
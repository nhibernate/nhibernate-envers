using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration
{
	[TestFixture]
	public class DifferentSchemaAndCatalogTest
	{
		private Cfg.Configuration configuration;
		private string assemblyName;
		private const string enversSchema = "enversSchema";
		private const string enversCatalog = "enversCatalog";

		[SetUp]
		public void Setup()
		{
			var assembly = GetType().Assembly;
			assemblyName = assembly.GetName().Name;
			configuration = new Cfg.Configuration()
							.SetEnversProperty(ConfigurationKey.DefaultSchema, enversSchema)
							.SetEnversProperty(ConfigurationKey.DefaultCatalog, enversCatalog)
							.Configure()
							.AddResource(assemblyName + ".Entities.Mapping.hbm.xml", assembly)
							.IntegrateWithEnvers();
		}


		[Test]
		public void VerifyAuditEntitySchemaAndCatalogName()
		{
			var table = configuration.GetClassMapping(assemblyName + ".Entities.StrTestEntity_AUD").Table;
			table.Schema
				.Should().Be.EqualTo(enversSchema);
			table.Catalog
				.Should().Be.EqualTo(enversCatalog);
		}

		[Test]
		public void VerifyMappedEntitySchemaAndCatalogName()
		{
			var table = configuration.GetClassMapping(typeof(StrTestEntity)).Table;
			table.Schema
				.Should().Be.Null();
			table.Catalog
				.Should().Be.Null();
		}

		[Test]
		public void VerifyRevInfoSchemaAndCatalogName()
		{
			var table = configuration.GetClassMapping(typeof(DefaultRevisionEntity)).Table;
			table.Schema
				.Should().Be.EqualTo(enversSchema);
			table.Catalog
				.Should().Be.EqualTo(enversCatalog);
		}
	}
}
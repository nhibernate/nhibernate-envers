using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration
{
	[TestFixture]
	public class SetPropertyTest
	{
		[Test]
		public void CanBeSetNhCoreWay()
		{
			var nhConfiguration = new Cfg.Configuration();
			nhConfiguration.SetProperty("nhibernate.envers.store_data_at_delete", "true");
			nhConfiguration.IntegrateWithEnvers();
			AuditConfiguration.GetFor(nhConfiguration).GlobalCfg.StoreDataAtDelete
				.Should().Be.True();
		}

		[Test]
		public void CanBeSetByConfigurationKey()
		{
			var nhConfiguration = new Cfg.Configuration();
			nhConfiguration.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
			nhConfiguration.IntegrateWithEnvers();
			AuditConfiguration.GetFor(nhConfiguration).GlobalCfg.StoreDataAtDelete
				.Should().Be.True();
		}

		[Test]
		public void CanBeSetByConfigurationKeyMethod()
		{
			var nhConfiguration = new Cfg.Configuration();
			ConfigurationKey.StoreDataAtDelete.SetUserValue(nhConfiguration, true);
			nhConfiguration.IntegrateWithEnvers();
			AuditConfiguration.GetFor(nhConfiguration).GlobalCfg.StoreDataAtDelete
				.Should().Be.True();
		}

		[Test]
		public void ShouldUseDefaultValue()
		{
			var nhConfiguration = new Cfg.Configuration();
			nhConfiguration.IntegrateWithEnvers();
			AuditConfiguration.GetFor(nhConfiguration).GlobalCfg.StoreDataAtDelete
				.Should().Be.False();
		}
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;
using NHibernate.Cfg;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Naming.UserSpecific
{
	public class EntityTest : TestBase
	{
		public EntityTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void VerifyTableName()
		{
			const string auditEntityName = "aNHibernate.Envers.Tests.Entities.IntTestEntity2";
			Cfg.GetClassMapping(auditEntityName).Table.Name.Should().Be.EqualTo("aIntTestEntity2");
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.TableNameStrategy, typeof(NamingStrategy));
			configuration.SetEnversProperty(ConfigurationKey.AuditTablePrefix, "a");
			configuration.SetEnversProperty(ConfigurationKey.AuditTableSuffix, "2");
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml" };
			}
		}
	}
}
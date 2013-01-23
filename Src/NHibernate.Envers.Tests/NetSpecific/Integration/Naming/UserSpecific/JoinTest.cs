using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Naming.UserSpecific
{
	public class JoinTest : TestBase
	{
		public JoinTest(string strategyType)
			: base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void VerifyTableName()
		{
			const string auditEntityName = "NHibernate.Envers.Tests.Integration.Join.JoinTestEntity_AUD";
			Cfg.GetClassMapping(auditEntityName).JoinIterator.First().Table.Name.Should().Be.EqualTo("Secondary2");
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.TableNameStrategy, typeof(NamingStrategy));
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Join.Mapping.hbm.xml" };
			}
		}
	}
}
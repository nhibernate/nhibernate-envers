using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Naming.UserSpecific
{
	public class DetachedOneToManyTest : TestBase
	{
		public DetachedOneToManyTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void VerifyTableName()
		{
			const string auditEntityName = "WikiPage_WikiImage_AUD";
			Cfg.GetClassMapping(auditEntityName).Table.Name.Should().Be.EqualTo("WikiPageWikiImage3");
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.TableNameStrategy, typeof(NamingStrategy));
		}


		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.HashCode.Mapping.hbm.xml" };
			}
		}
	}
}
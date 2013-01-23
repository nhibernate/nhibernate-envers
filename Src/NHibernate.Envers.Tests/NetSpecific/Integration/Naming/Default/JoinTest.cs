using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Naming.Default
{
	public class JoinTest : TestBase
	{
		public JoinTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void VerifyTableName()
		{
			const string auditEntityName = "NHibernate.Envers.Tests.Integration.Join.JoinTestEntity_AUD";
			Cfg.GetClassMapping(auditEntityName).JoinIterator.First().Table.Name.Should().Be.EqualTo("Secondary_AUD");
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
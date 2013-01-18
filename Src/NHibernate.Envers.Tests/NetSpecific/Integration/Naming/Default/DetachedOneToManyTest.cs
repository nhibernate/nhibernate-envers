using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Naming.Default
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
		public void VerifyJoinTableName()
		{
			const string auditEntityName = "WikiPage_WikiImage_AUD";
			Cfg.GetClassMapping(auditEntityName).Table.Name.Should().Be.EqualTo("WikiPage_WikiImage_AUD");
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
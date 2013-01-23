using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Naming.Default
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
			const string auditEntityName = "NHibernate.Envers.Tests.Entities.IntTestEntity_AUD";
			Cfg.GetClassMapping(auditEntityName).Table.Name.Should().Be.EqualTo("IntTestEntity_AUD");
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
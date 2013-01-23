using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Naming.Default
{
	public class CollectionTest : TestBase
	{
		public CollectionTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void VerifyTableName()
		{
			const string auditEntityName = "Enums1_AUD";
			Cfg.GetClassMapping(auditEntityName).Table.Name.Should().Be.EqualTo("Enums1_AUD");
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Collection.Mapping.hbm.xml" }; }
		}
	}
}
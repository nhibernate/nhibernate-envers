using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public class NoneAuditedTest : TestBase
	{
		public NoneAuditedTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test] 
		public void ShouldNotCreateRevTable()
		{
			Cfg.ClassMappings.Count
				.Should().Be.EqualTo(1);
			Cfg.ClassMappings.First().ClassName
				.Should().Contain("BasicTestEntity3");
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Basic.NonAuditedEntity.hbm.xml" };
			}
		}
	}
}
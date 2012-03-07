using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class ConfByStringGenericTest : ConfByStringBaseTest
	{
		[SetUp]
		public void Setup()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit<FieldEntity>()
				.Exclude("data1")
				.Exclude("data2");
			cfg.Audit<NotAuditedOwnerEntity>()
				.ExcludeRelationData("RelationField");
			metas = cfg.CreateMetaData(null);
		}

		[Test]
		public void IncorrectString()
		{
			var cfg = new FluentConfiguration();
			Assert.Throws<FluentException>(() =>
															cfg.Audit<FieldEntity>()
																 .Exclude("data3")
													  );
		}
	}
}

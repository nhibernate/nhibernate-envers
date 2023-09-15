using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride
{
	public partial class MixedOverrideTest : TestBase
	{
		private int mixedEntityId;

		public MixedOverrideTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var mixedEntity = new MixedOverrideEntity {Number1 = 1, Str1 = "data 1", Str2 = "data 2"};
				mixedEntityId = (int) Session.Save(mixedEntity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyAuditedProperty()
		{
			var table = mixedTable();
			table.GetColumn(new Column("Number1"))
				.Should().Not.Be.Null();
			table.GetColumn(new Column("Str2"))
				.Should().Not.Be.Null();
		}

		[Test]
		public void VerifyNotAuditedProperty()
		{
			mixedTable().GetColumn(new Column("Str1"))
				.Should().Be.Null();
		}

		[Test]
		public void VerifyHistoryOfMixedEntity()
		{
			var ver1 = new MixedOverrideEntity {Id = mixedEntityId, Number1 = 1, Str1 = null, Str2 = "data 2"};
			AuditReader().Find<MixedOverrideEntity>(mixedEntityId, 1)
				.Should().Be.EqualTo(ver1);
		}

		private Table mixedTable()
		{
			return Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride.MixedOverrideEntity_AUD").Table;
		}
	}
}
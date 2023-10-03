using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride
{
	public partial class AuditPropertyOverrideTest : TestBase
	{
		private int propertyEntityId;
		private int transitiveEntityId;
		private int auditedEntityId;

		public AuditPropertyOverrideTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				var propertyEntity = new PropertyOverrideTestEntity {Number1 = 1, Str1 = "data 1", Str2 = "data 2"};
				propertyEntityId = (int) Session.Save(propertyEntity);
				tx.Commit();
			}
			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				var transitiveEntity = new TransitiveOverrideTestEntity {Number1 = 1, Number2 = 2, Str1 = "data 1", Str2 = "data 2", Str3 = "data 3"};
				transitiveEntityId = (int) Session.Save(transitiveEntity);
				tx.Commit();
			}
			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				var auditedEntity = new AuditedSpecialEntity {Number1 = 1, Str1 = "data 1", Str2 = "data 2"};
				auditedEntityId = (int) Session.Save(auditedEntity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyNotAuditedProperty()
		{
			propertyTable().GetColumn(new Column("Str1"))
				.Should().Be.Null();
		}

		[Test]
		public void VerifyAuditedProperty()
		{
			propertyTable().GetColumn(new Column("Number1"))
				.Should().Not.Be.Null();
			transitiveTable().GetColumn(new Column("Number2"))
				.Should().Not.Be.Null();
			auditedTable().GetColumn(new Column("Str1"))
				.Should().Not.Be.Null();
		}

		[Test]
		public void VerifyTransitiveAuditedProperty()
		{
			var table = transitiveTable();
			table.GetColumn(new Column("Number1"))
				.Should().Not.Be.Null();
			table.GetColumn(new Column("Str1"))
				.Should().Not.Be.Null();
		}

		[Test]
		public void VerifyHistoryOfPropertyOverrideEntity()
		{
			var ver1 = new PropertyOverrideTestEntity {Id = propertyEntityId, Number1 = 1, Str1 = null, Str2 = "data 2"};
			AuditReader().Find<PropertyOverrideTestEntity>(propertyEntityId, 1)
				.Should().Be.EqualTo(ver1);
		}

		[Test]
		public void VerifyHistoryOfTransitiveOverrideEntity()
		{
			var ver1 = new TransitiveOverrideTestEntity { Id = transitiveEntityId, Number1 = 1, Number2 = 2, Str1 = "data 1", Str2 = "data 2", Str3 = "data 3"};
			AuditReader().Find<TransitiveOverrideTestEntity>(transitiveEntityId, 2)
				.Should().Be.EqualTo(ver1);
		}

		[Test]
		public void VerifyHistoryOfAuditedSpecialEntity()
		{
			var ver1 = new AuditedSpecialEntity { Id = auditedEntityId, Number1 = 0, Str1 = "data 1", Str2 = "data 2" };
			AuditReader().Find<AuditedSpecialEntity>(auditedEntityId, 3)
				.Should().Be.EqualTo(ver1);
		}

		private Table auditedTable()
		{
			return Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride.AuditedSpecialEntity_AUD").Table;
		}

		private Table transitiveTable()
		{
			return Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride.TransitiveOverrideTestEntity_AUD").Table;
		}

		private Table propertyTable()
		{
			return Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride.PropertyOverrideTestEntity_AUD").Table;
		}
	}
}
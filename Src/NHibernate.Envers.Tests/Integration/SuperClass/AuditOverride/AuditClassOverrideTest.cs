using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride
{
	public partial class AuditClassOverrideTest : TestBase
	{
		private int classAuditedEntityId;
		private int classNotAuditedEntityId;

		public AuditClassOverrideTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				var classOverrideAuditedEntity = new ClassOverrideAuditedEntity {Number1 = 1, Str1 = "data 1", Str2 = "data 2"};
				classAuditedEntityId = (int) Session.Save(classOverrideAuditedEntity);
				tx.Commit();
			}
			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				var classNotOverrideAuditedEntity = new ClassOverrideNotAuditedEntity { Number1 = 1, Str1 = "data 1", Str2 = "data 2" };
				classNotAuditedEntityId = (int)Session.Save(classNotOverrideAuditedEntity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyAuditedProperty()
		{
			var auditTable = auditedTable();
			auditTable.GetColumn(new Column("Number1"))
				.Should().Not.Be.Null();
			auditTable.GetColumn(new Column("Str1"))
				.Should().Not.Be.Null();
			auditTable.GetColumn(new Column("Str2"))
				.Should().Not.Be.Null();
			notAuditedTable().GetColumn(new Column("Str2"))
				.Should().Not.Be.Null();
		}

		[Test]
		public void VerifyNotAuditedProperty()
		{
			var notAuditTable = notAuditedTable();
			notAuditTable.GetColumn(new Column("Number1"))
				.Should().Be.Null();
			notAuditTable.GetColumn(new Column("Str1"))
				.Should().Be.Null();
		}

		[Test]
		public void VerifyHistoryOfClassOverrideAuditedEntity()
		{
			var ver1 = new ClassOverrideAuditedEntity {Id = classAuditedEntityId, Number1 = 1, Str1 = "data 1", Str2 = "data 2"};
			AuditReader().Find<ClassOverrideAuditedEntity>(classAuditedEntityId, 1)
				.Should().Be.EqualTo(ver1);
		}

		[Test]
		public void VerifyHistoryOfClassOverrideNotAuditedEntity()
		{
			var ver1 = new ClassOverrideNotAuditedEntity { Id = classNotAuditedEntityId, Number1 = 0, Str1 = null, Str2 = "data 2" };
			AuditReader().Find<ClassOverrideNotAuditedEntity>(classNotAuditedEntityId, 2)
				.Should().Be.EqualTo(ver1);
		}

		private Table auditedTable()
		{
			return Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride.ClassOverrideAuditedEntity_AUD").Table;
		}

		private Table notAuditedTable()
		{
			return Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride.ClassOverrideNotAuditedEntity_AUD").Table;
		}
	}
}
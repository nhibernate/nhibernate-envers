using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Components
{
	public partial class PropertiesGroupTest : TestBase
	{
		private long auditedId;
		private long notAuditedId;

		public PropertiesGroupTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				var ent = new UniquePropsEntity { Data1 = "data1", Data2 = "data2" };
				auditedId = (long)Session.Save(ent);
				tx.Commit();
			}

			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				var entNotAudited = new UniquePropsNotAuditedEntity { Data1 = "data3", Data2 = "data4" };
				notAuditedId = (long) Session.Save(entNotAudited);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyAuditTableColumnsForEntity()
		{
			var table = Cfg.GetClassMapping(TestAssembly + ".Entities.Components.UniquePropsEntity_AUD").Table;
			table.GetColumn(new Column("Data1"))
				.Should().Not.Be.Null();
			table.GetColumn(new Column("Data2"))
				.Should().Not.Be.Null();
		}

		[Test]
		public void VerifyAuditTableColumnsForNotAuditedEntity()
		{
			var table = Cfg.GetClassMapping(TestAssembly + ".Entities.Components.UniquePropsNotAuditedEntity_AUD").Table;
			table.GetColumn(new Column("Data1"))
				.Should().Not.Be.Null();
			table.GetColumn(new Column("Data2"))
				.Should().Be.Null();
		}

		[Test]
		public void VerifyHistoryOfUniquePropsEntity()
		{
			var expected = new UniquePropsEntity {Id = auditedId, Data1 = "data1", Data2 = "data2"};
			AuditReader().Find<UniquePropsEntity>(auditedId, 1)
				.Should().Be.EqualTo(expected);
		}

		[Test]
		public void VerifyHistoryOfUniquePropsNotAuditedEntity()
		{
			var expected = new UniquePropsNotAuditedEntity { Id = notAuditedId, Data1 = "data3"};
			AuditReader().Find<UniquePropsNotAuditedEntity>(notAuditedId, 2)
				.Should().Be.EqualTo(expected);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Components.UniqueProps.hbm.xml" };
			}
		}
	}
}
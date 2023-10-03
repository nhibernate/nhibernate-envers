using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class ColumnScalePrecisionTest : TestBase
	{
		private long id;

		public ColumnScalePrecisionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var entity = new ScalePrecisionEntity {Number = 13};
				id = (long) Session.Save(entity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyColumnScalePrecision()
		{
			var auditTable = Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.Basic.ScalePrecisionEntity_AUD").Table;
			var originalTable = Cfg.GetClassMapping(typeof (ScalePrecisionEntity)).Table;
			var testColumn = new Column("thenumber");
			var scalePrecisionAuditColumn = auditTable.GetColumn(testColumn);
			var scalePrecisionColumn = originalTable.GetColumn(testColumn);

			scalePrecisionAuditColumn.Should().Not.Be.Null();
			scalePrecisionAuditColumn.Precision.Should().Be.EqualTo(scalePrecisionColumn.Precision);
			scalePrecisionAuditColumn.Scale.Should().Be.EqualTo(scalePrecisionColumn.Scale);
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (ScalePrecisionEntity), id)
				.Should().Have.SameSequenceAs(1);
		}

		[Test]
		public void VerifyHistoryOfScalePrecisionEntity()
		{
			var ver1 = new ScalePrecisionEntity {Id = id, Number = 13};
			AuditReader().Find<ScalePrecisionEntity>(id, 1)
				.Should().Be.EqualTo(ver1);
		}

		protected override System.Collections.Generic.IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Basic.ScalePrecision.hbm.xml" };
			}
		}
	}
}
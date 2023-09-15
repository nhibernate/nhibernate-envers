using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Naming.Quotation
{
	public partial class QuotedFieldsTest : TestBase
	{
		private long qfeId1;
		private long qfeId2;

		public QuotedFieldsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var qfe1 = new QuotedFieldsEntity {Data1 = "data1", Data2 = 1};
			var qfe2 = new QuotedFieldsEntity {Data1 = "data2", Data2 = 2};

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				qfeId1 = (long) Session.Save(qfe1);
				qfeId2 = (long) Session.Save(qfe2);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				qfe1.Data1 = "data changed";
				tx.Commit();
			}

			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				qfe2.Data2 = 3;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			AuditReader().GetRevisions(typeof(QuotedFieldsEntity), qfeId1)
				.Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof(QuotedFieldsEntity), qfeId2)
				.Should().Have.SameSequenceAs(1, 3);
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ver1 = new QuotedFieldsEntity { Id = qfeId1, Data1 = "data1", Data2 = 1 };
			var ver2 = new QuotedFieldsEntity { Id = qfeId1, Data1 = "data changed", Data2 = 1 };

			AuditReader().Find<QuotedFieldsEntity>(qfeId1, 1).Should().Be.EqualTo(ver1);
			AuditReader().Find<QuotedFieldsEntity>(qfeId1, 2).Should().Be.EqualTo(ver2);
			AuditReader().Find<QuotedFieldsEntity>(qfeId1, 3).Should().Be.EqualTo(ver2);
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var ver1 = new QuotedFieldsEntity { Id = qfeId2, Data1 = "data2", Data2 = 2 };
			var ver2 = new QuotedFieldsEntity { Id = qfeId2, Data1 = "data2", Data2 = 3 };

			AuditReader().Find<QuotedFieldsEntity>(qfeId2, 1).Should().Be.EqualTo(ver1);
			AuditReader().Find<QuotedFieldsEntity>(qfeId2, 2).Should().Be.EqualTo(ver1);
			AuditReader().Find<QuotedFieldsEntity>(qfeId2, 3).Should().Be.EqualTo(ver2);
		}

		[Test]
		public void VerifyEscapeEntityField()
		{
			var table = Cfg.GetClassMapping(TestAssembly + ".Integration.Naming.Quotation.QuotedFieldsEntity_AUD").Table;
			var column1 = columnByName(table, "select");
			var column2 = columnByName(table, "from");
			var column3 = columnByName(table, "where");
			column1.IsQuoted.Should().Be.True();
			column2.IsQuoted.Should().Be.True();
			column3.IsQuoted.Should().Be.True();
		}

		private Column columnByName(Table table, string columnName)
		{
			foreach (var column in table.ColumnIterator)
			{
				if (column.Name.Equals(columnName))
					return column;
			}
			Assert.Fail("Can't find column " + columnName);
			return null;
		}
	}
}
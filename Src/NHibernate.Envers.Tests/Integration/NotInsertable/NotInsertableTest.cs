using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.NotInsertable
{
	public partial class NotInsertableTest : TestBase
	{
		private int id1;

		public NotInsertableTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var dte = new NotInsertableTestEntity { Data = "data1" };

			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(dte);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				dte.Data = "data2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(NotInsertableTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new NotInsertableTestEntity { Id = id1, Data = "data1", DataCopy = "data1" };
			var ver2 = new NotInsertableTestEntity { Id = id1, Data = "data2", DataCopy = "data2" };

			var rev1 = AuditReader().Find<NotInsertableTestEntity>(id1, 1);
			var rev2 = AuditReader().Find<NotInsertableTestEntity>(id1, 2);

			Assert.AreEqual(ver1, rev1);
			Assert.AreEqual(ver2, rev2);
		}
	}
}
using System;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Data
{
	public partial class DateTest : TestBase
	{
		private int id1;

		public DateTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var dte = new DateTestEntity { Date = new DateTime(2000,1,2,3,4,5) };
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(dte);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				dte.Date = new DateTime(2001,2,3,4,5,6);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(DateTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new DateTestEntity {Id = id1, Date = new DateTime(2000,1,2,3,4,5)};
			var ver2 = new DateTestEntity { Id = id1, Date = new DateTime(2001,2,3,4,5,6) };

			Assert.AreEqual(ver1, AuditReader().Find<DateTestEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<DateTestEntity>(id1, 2));
		}
	}
}
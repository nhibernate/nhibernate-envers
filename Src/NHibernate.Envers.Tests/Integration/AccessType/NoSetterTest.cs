using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.AccessType
{
	public partial class NoSetterTest : TestBase
	{
		private int id1;

		public NoSetterTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var nse = new NoSetterEntity("data");
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(nse);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				nse.WriteData("data2");
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(NoSetterEntity), id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var rev1 = AuditReader().Find<NoSetterEntity>(id1, 1);
			var rev2 = AuditReader().Find<NoSetterEntity>(id1, 2);

			Assert.AreEqual("data", rev1.Data);
			Assert.AreEqual("data2", rev2.Data);
		}
	}
}
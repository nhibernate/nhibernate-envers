using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.AccessType
{
	public partial class MixedAccessTest : TestBase
	{
		private int id1;

		public MixedAccessTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var mate = new MixedAccessEntity("data");
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(mate);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				mate.WriteData("data2");
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(MixedAccessEntity), id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new MixedAccessEntity(id1, "data");
			var ver2 = new MixedAccessEntity(id1, "data2");

			var rev1 = AuditReader().Find<MixedAccessEntity>(id1, 1);
			var rev2 = AuditReader().Find<MixedAccessEntity>(id1, 2);

			Assert.IsTrue(rev1.IsDataSet);
			Assert.IsTrue(rev2.IsDataSet);

			Assert.AreEqual(ver1, rev1);
			Assert.AreEqual(ver2, rev2);
		}
	}
}
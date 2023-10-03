using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Data
{
	public partial class LobsTest : TestBase
	{
		private int id1;

		public LobsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var lte = new LobTestEntity {StringLob = "abc", ByteLob = new byte[] {0, 1, 2}};
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(lte);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				lte.StringLob = "def";
				lte.ByteLob = new byte[] {3, 4, 5};
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(LobTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new LobTestEntity { Id = id1, StringLob = "abc", ByteLob = new byte[] { 0, 1, 2 } };
			var ver2 = new LobTestEntity { Id = id1, StringLob = "def", ByteLob = new byte[] { 3, 4, 5 } };

			Assert.AreEqual(ver1, AuditReader().Find<LobTestEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<LobTestEntity>(id1, 2));
		}
	}
}
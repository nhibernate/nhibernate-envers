using System.Linq;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Join
{
	public partial class NamingJoinTest : TestBase
	{
		private int id;

		public NamingJoinTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ste = new JoinWithAuditNameEntity {S1 = "a", S2 = "1"};
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(ste);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ste.S1 = "b";
				ste.S2 = "2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(JoinWithAuditNameEntity), id));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new JoinWithAuditNameEntity { Id = id, S1 = "a", S2 = "1" };
			var ver2 = new JoinWithAuditNameEntity { Id = id, S1 = "b", S2 = "2" };

			Assert.AreEqual(ver1, AuditReader().Find<JoinWithAuditNameEntity>(id, 1));
			Assert.AreEqual(ver2, AuditReader().Find<JoinWithAuditNameEntity>(id, 2));
		}

		[Test]
		public void VerifyTableNames()
		{
			var auditName = TestAssembly + ".Integration.Join.JoinWithAuditNameEntity_AUD";
			var joinTableAudit = Cfg.GetClassMapping(auditName).JoinIterator.First().Table.Name;
			Assert.AreEqual("sec_versions", joinTableAudit);
		}

	}
}
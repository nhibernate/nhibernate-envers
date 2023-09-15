using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Versioning
{
	public partial class NoOptimisticLockAuditingTest : TestBase
	{
		private const int id = 17;

		public NoOptimisticLockAuditingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ole = new OptimisticLockEntity { Id = id, Str = "X" };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ole);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ole.Str = "Y";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(OptimisticLockEntity), id));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new OptimisticLockEntity { Id = id, Str = "X" };
			var ver2 = new OptimisticLockEntity { Id = id, Str = "Y" };

			Assert.AreEqual(ver1, AuditReader().Find<OptimisticLockEntity>(id, 1));
			Assert.AreEqual(ver2, AuditReader().Find<OptimisticLockEntity>(id, 2));
		}

		[Test]
		public void VerifyVersionedIsNotAudited()
		{
			var versionOf1 = AuditReader().Find<OptimisticLockEntity>(id, 1).Version;
			var versionOf2 = AuditReader().Find<OptimisticLockEntity>(id, 2).Version;

			Assert.AreEqual(0, versionOf1);
			Assert.AreEqual(0, versionOf2);
		}

		[Test]
		public void VerifyMapping()
		{
			var auditName = TestAssembly + ".Integration.Versioning.OptimisticLockEntity_AUD";
			foreach (var property in Cfg.GetClassMapping(auditName).PropertyIterator)
			{
				Assert.AreNotEqual("Version", property.Name);
			}
		}
	}
}
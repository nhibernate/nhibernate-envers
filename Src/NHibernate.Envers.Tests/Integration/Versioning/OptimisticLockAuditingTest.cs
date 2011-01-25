using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Versioning
{
	[TestFixture]
	public class OptimisticLockAuditingTest : TestBase
	{
		private const int id =47;

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetProperty("envers.do_not_audit_optimistic_locking_field", "false");
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
		public void VerifyVersionedIsAudited()
		{
			var versionOf1 = AuditReader().Find<OptimisticLockEntity>(id, 1).Version;
			var versionOf2 = AuditReader().Find<OptimisticLockEntity>(id, 2).Version;

			Assert.AreEqual(versionOf1 +1, versionOf2);
		}

		[Test]
		public void VerifyMapping()
		{
			const string auditName = TestAssembly + ".Integration.Versioning.OptimisticLockEntity_AUD";
			var versionColumnExists = false;
			foreach (var property in Cfg.GetClassMapping(auditName).PropertyIterator)
			{
				if (property.Name.Equals("Version"))
					versionColumnExists = true;
			}
			Assert.IsTrue(versionColumnExists);
		}
	}
}
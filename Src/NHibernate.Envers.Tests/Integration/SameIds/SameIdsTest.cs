using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.SameIds
{
	public partial class SameIdsTest : TestBase
	{
		public SameIdsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var site1 = new SameIdTestEntity1 {Id = 1, Str1 = "str1"};
			var site2 = new SameIdTestEntity2 {Id = 1, Str1 = "str1"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(site1);
				Session.Save(site2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				site1.Str1 = "str2";
				site2.Str1 = "str2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(SameIdTestEntity1), 1));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(SameIdTestEntity2), 1));
		}

		[Test]
		public void VerifyHistoryOfSite1()
		{
			var ver1 = new SameIdTestEntity1 { Id = 1, Str1 = "str1" };
			var ver2 = new SameIdTestEntity1 { Id = 1, Str1 = "str2" };

			Assert.AreEqual(ver1, AuditReader().Find<SameIdTestEntity1>(1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<SameIdTestEntity1>(1, 2));
		}

		[Test]
		public void VerifyHistoryOfSite2()
		{
			var ver1 = new SameIdTestEntity2 { Id = 1, Str1 = "str1" };
			var ver2 = new SameIdTestEntity2 { Id = 1, Str1 = "str2" };

			Assert.AreEqual(ver1, AuditReader().Find<SameIdTestEntity2>(1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<SameIdTestEntity2>(1, 2));
		}
	}
}
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class NullPropertiesTest : TestBase
	{
		private int id1;
		private int id2;

		public NullPropertiesTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		private int addNewEntity(string str, long lng)
		{
			using(var tx = Session.BeginTransaction())
			{
				var bte = new BasicTestEntity1 {Str1 = str, Long1 = lng};
				Session.Save(bte);
				tx.Commit();
				return bte.Id;
			}
		}

		private void modifyEntity(int id, string str, long lng)
		{
			using(var tx = Session.BeginTransaction())
			{
				var bte = Session.Get<BasicTestEntity1>(id);
				bte.Long1 = lng;
				bte.Str1 = str;
				tx.Commit();
			}
		}

		protected override void Initialize()
		{
			id1 = addNewEntity("x", 1); //rev 1
			id2 = addNewEntity(null, 20); //rev 2

			modifyEntity(id1, null, 1); // rev 3
			modifyEntity(id2, "y2", 20); // rev 4
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(BasicTestEntity1), id1));
			CollectionAssert.AreEquivalent(new[] { 2, 4 }, AuditReader().GetRevisions(typeof(BasicTestEntity1), id2));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ver1 = new BasicTestEntity1 { Id = id1, Str1 = "x", Long1 = 1 };
			var ver2 = new BasicTestEntity1 { Id = id1, Long1 = 1 };

			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id1, 1));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id1, 2));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id1, 3));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id1, 4));
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var ver1 = new BasicTestEntity1 { Id = id2, Long1 = 20 };
			var ver2 = new BasicTestEntity1 { Id = id2, Str1 = "y2", Long1 = 20 };

			Assert.IsNull(AuditReader().Find<BasicTestEntity1>(id2, 1));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id2, 2));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id2, 3));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id2, 4));
		}
	}
}
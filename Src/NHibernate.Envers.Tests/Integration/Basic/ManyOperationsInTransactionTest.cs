using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class ManyOperationsInTransactionTest : TestBase
	{
		private int id1;
		private int id2;
		private int id3;

		public ManyOperationsInTransactionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var bte1 = new BasicTestEntity1 {Str1 = "x", Long1 = 1};
			var bte2 = new BasicTestEntity1 {Str1 = "y", Long1 = 20};
			var bte3 = new BasicTestEntity1 { Str1 = "z", Long1 = 300};
			//revision 1
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(bte1);
				id2 = (int) Session.Save(bte2);
				tx.Commit();
			}
			//revision 2
			using(var tx = Session.BeginTransaction())
			{
				id3 = (int) Session.Save(bte3);
				bte1.Str1 = "x2";
				bte2.Long1 = 21;
				tx.Commit();
			}
			//revision 3
			using(var tx = Session.BeginTransaction())
			{
				bte2.Str1 = "y3";
				bte2.Long1 = 22;
				bte3.Str1 = "z3";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BasicTestEntity1), id1));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(BasicTestEntity1), id2));
			CollectionAssert.AreEquivalent(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(BasicTestEntity1), id3));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ver1 = new BasicTestEntity1 { Id = id1, Str1 = "x", Long1 = 1 };
			var ver2 = new BasicTestEntity1 { Id = id1, Str1 = "x2", Long1 = 1 };

			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id1, 2));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id1, 3));
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var ver1 = new BasicTestEntity1 { Id = id2, Str1 = "y", Long1 = 20 };
			var ver2 = new BasicTestEntity1 { Id = id2, Str1 = "y", Long1 = 21 };
			var ver3 = new BasicTestEntity1 { Id = id2, Str1 = "y3", Long1 = 22 };

			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id2, 1));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id2, 2));
			Assert.AreEqual(ver3, AuditReader().Find<BasicTestEntity1>(id2, 3));
		}

		[Test]
		public void VerifyHistoryOf3()
		{
			var ver1 = new BasicTestEntity1 { Id = id3, Str1 = "z", Long1 = 300 };
			var ver2 = new BasicTestEntity1 { Id = id3, Str1 = "z3", Long1 = 300 };

			Assert.IsNull(AuditReader().Find<BasicTestEntity1>(id3, 1));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id3, 2));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id3, 3));
		}
	}
}
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class DeleteTest : TestBase
	{
		private int id1;
		private int id2;
		private int id3;

		public DeleteTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var bte1 = new BasicTestEntity2 { Str1 = "x", Str2 = "a" };
			var bte2 = new BasicTestEntity2 { Str1 = "y", Str2 = "b" };
			var bte3 = new BasicTestEntity2 { Str1 = "z", Str2 = "c" };
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(bte1);
				id2 = (int)Session.Save(bte2);
				id3 = (int)Session.Save(bte3);
				tx.Commit();
			}
			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				bte1.Str1 = "x2";
				bte2.Str2 = "b2";
				Session.Delete(bte3);
				tx.Commit();
			}
			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(bte2);
				tx.Commit();
			}
			//revision 4
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(bte1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount() 
		{
			CollectionAssert.AreEquivalent(new[] {1, 2, 4}, AuditReader().GetRevisions(typeof(BasicTestEntity2),id1));
			CollectionAssert.AreEquivalent(new[] {1, 3}, AuditReader().GetRevisions(typeof(BasicTestEntity2),id2));
			CollectionAssert.AreEquivalent(new[] {1, 2}, AuditReader().GetRevisions(typeof(BasicTestEntity2),id3));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ver1 = new BasicTestEntity2 {Id = id1, Str1 = "x"};
			var ver2 = new BasicTestEntity2 {Id = id1, Str1 = "x2"};
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity2>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity2>(id1, 2));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity2>(id1, 3));
			Assert.IsNull(AuditReader().Find<BasicTestEntity2>(id1, 4));
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var ver1 = new BasicTestEntity2 { Id = id2, Str1 = "y" };
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity2>(id2, 1));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity2>(id2, 2));
			Assert.IsNull(AuditReader().Find<BasicTestEntity2>(id2, 3));
			Assert.IsNull(AuditReader().Find<BasicTestEntity2>(id2, 4));
		}

		[Test]
		public void VerifyHistoryOf3()
		{
			var ver1 = new BasicTestEntity2 { Id = id3, Str1 = "z" };
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity2>(id3, 1));
			Assert.IsNull(AuditReader().Find<BasicTestEntity2>(id3, 2));
			Assert.IsNull(AuditReader().Find<BasicTestEntity2>(id3, 3));
			Assert.IsNull(AuditReader().Find<BasicTestEntity2>(id3, 4));
		}
	}
}
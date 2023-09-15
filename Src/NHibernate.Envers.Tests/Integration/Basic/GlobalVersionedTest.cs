using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class GlobalVersionedTest : TestBase
	{
		private int id1;

		public GlobalVersionedTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var bte1 = new BasicTestEntity4 {Str1 = "x", Str2 = "y"};
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(bte1);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				bte1.Str1 = "a";
				bte1.Str2 = "b";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BasicTestEntity4), id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new BasicTestEntity4 { Id = id1, Str1 = "x", Str2 = "y" };
			var ver2 = new BasicTestEntity4 { Id = id1, Str1 = "a", Str2 = "b" };

			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity4>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity4>(id1, 2));
		}
	}
}
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class UnversionedPropertiesChangeTest : TestBase
	{
		private int id1;

		public UnversionedPropertiesChangeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		private int addNewEntity(string str1, string str2)
		{
			using (var tx = Session.BeginTransaction())
			{
				var bte = new BasicTestEntity2 { Str1 = str1, Str2 = str2 };
				Session.Save(bte);
				tx.Commit();
				return bte.Id;
			}
		}

		private void modifyEntity(int id, string str1, string str2)
		{
			using (var tx = Session.BeginTransaction())
			{
				var bte = Session.Get<BasicTestEntity2>(id);
				bte.Str1 = str1;
				bte.Str2 = str2;
				tx.Commit();
			}
		}

		protected override void Initialize()
		{
			id1 = addNewEntity("x", "a"); //rev 1
			modifyEntity(id1, "x", "a"); // no rev
			modifyEntity(id1, "y", "b"); // rev 2
			modifyEntity(id1, "y", "c"); // no rev
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(BasicTestEntity2), id1));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ver1 = new BasicTestEntity2 { Id = id1, Str1 = "x"};
			var ver2 = new BasicTestEntity2 { Id = id1, Str1 = "y"};

			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity2>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity2>(id1, 2));
		}
	}
}
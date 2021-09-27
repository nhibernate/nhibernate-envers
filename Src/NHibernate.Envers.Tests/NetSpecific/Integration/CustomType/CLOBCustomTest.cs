using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomType
{
	public class CLOBCustomTest : TestBase
	{
		private int ccte_id;

		public CLOBCustomTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ccte = new CLOBCustomTypeEntity();

			using (var tx = Session.BeginTransaction())
			{
				ccte.Str = "U";
				Session.Save(ccte);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ccte.Str = "V";
				Session.Save(ccte);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ccte.Str = new string('Y', 20000);
				Session.Save(ccte);
				tx.Commit();
			}
			ccte_id = ccte.Id;
		}

		protected override string TestShouldNotRunMessage()
		{
			return Dialect is MySQLDialect ? "Not applicable for MySQL" : null;
		}


		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(CLOBCustomTypeEntity), ccte_id));
		}

		[Test]
		public void VerifyHistoryOfCcte()
		{
			var rev1 = AuditReader().Find<CLOBCustomTypeEntity>(ccte_id, 1);
			var rev2 = AuditReader().Find<CLOBCustomTypeEntity>(ccte_id, 2);

			Assert.AreEqual("U", rev1.Str);
			Assert.AreEqual("V", rev2.Str);
		}

		[Test]
		public void VerifyLengthOfCcte()
		{
			var rev3 = AuditReader().Find<CLOBCustomTypeEntity>(ccte_id, 3);

			Assert.AreEqual(20000, rev3.Str.Length);
		}
	}
}
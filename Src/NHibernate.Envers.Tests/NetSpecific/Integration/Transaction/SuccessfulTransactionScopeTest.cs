using System.Collections.Generic;
using System.Transactions;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Transaction
{
	[TestFixture, Ignore("Not working. See NHE-13")]
	public class SuccessfulTransactionScopeTest : TestBase
	{
		private int id;

		public SuccessfulTransactionScopeTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new IntTestEntity { Number = 10 };
			using (var tx = new TransactionScope())
			{
				id = (int)Session.Save(entity);
				tx.Complete();
			}
			using (var tx = new TransactionScope())
			{
				entity.Number = 20;
				tx.Complete();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(IntTestEntity),id));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new IntTestEntity { Id = id, Number = 10 };
			var ver2 = new IntTestEntity { Id = id, Number = 20 };

			Assert.AreEqual(ver1, AuditReader().Find<IntTestEntity>(id, 1));
			Assert.AreEqual(ver2, AuditReader().Find<IntTestEntity>(id, 2));
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml" }; }
		}
	}
}
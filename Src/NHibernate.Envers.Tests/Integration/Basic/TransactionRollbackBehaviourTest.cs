using System.Collections.Generic;
using System.Linq;
using NHibernate.Dialect;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public class TransactionRollbackBehaviourTest : TestBase
	{
		private int rollbackId;
		private int committedId;

		public TransactionRollbackBehaviourTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var iteRollback = new IntTestEntity {Number = 30};
				rollbackId = (int) Session.Save(iteRollback);
				tx.Rollback();
			}
			using (var tx = Session.BeginTransaction())
			{
				var iteCommitted = new IntTestEntity {Number = 50};
				committedId = (int) Session.Save(iteCommitted);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyAuditRecordsRollback()
		{
			if(Dialect is Oracle8iDialect)
				Assert.Ignore("Fails due to NH Core issue https://nhibernate.jira.com/browse/NH-3304");
			AuditReader().GetRevisions(typeof (IntTestEntity), committedId).Count().Should().Be.EqualTo(1);
			AuditReader().GetRevisions(typeof (IntTestEntity), rollbackId).Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"Entities.Mapping.hbm.xml"}; }
		}
	}
}
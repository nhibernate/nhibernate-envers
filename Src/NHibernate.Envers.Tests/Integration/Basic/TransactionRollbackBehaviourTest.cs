using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class TransactionRollbackBehaviourTest : TestBase
	{
		private int rollbackId;
		private int committedId;

		public TransactionRollbackBehaviourTest(AuditStrategyForTest strategyType) : base(strategyType)
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
		public void CommittedEntityShouldHaveAuditRecord()
		{
			AuditReader().GetRevisions(typeof(IntTestEntity), committedId).Count().Should().Be.EqualTo(1);
		}

		[Test]
		public void RollbackedEntityShouldHaveAuditRecordIfPersisted()
		{
			//NH might persist rollbacked entity
			//https://nhibernate.jira.com/browse/NH-3304
			var revisions = AuditReader().GetRevisions(typeof (IntTestEntity), rollbackId);
			var entity = Session.Get<IntTestEntity>(rollbackId);
			if (entity == null)
			{
				revisions.Should().Be.Empty();
			}
			else
			{
				//if code generated poid, the session is still dirty when second transaction starts
				// eg oracle dialects ends up here in this test
				revisions.Count().Should().Be.EqualTo(1);
			}
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"Entities.Mapping.hbm.xml"}; }
		}
	}
}
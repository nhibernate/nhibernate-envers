using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.TransactionUnexpectedFlush
{
	public class TransactionUnexpectedFlushTest : TestBase
	{
		public TransactionUnexpectedFlushTest(AuditStrategyForTest strategyType)
			: base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings => new[] {"NetSpecific.Integration.TransactionUnexpectedFlush.Mapping.hbm.xml"};

		protected override void Initialize()
        {
		}

		[Test]
		public void GetCurrentRevision_PersistIsTrue_ShouldNotFlushUncommitedChanges()
		{
			// Arrange
			var entity = new Entity { Id = Guid.NewGuid(), Name = "entity name" };

			int notCommitedTransactionEntitiesCount = 0;

			// Act
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);

				AuditReader().GetCurrentRevision(true);

				notCommitedTransactionEntitiesCount = Session.Query<DefaultRevisionEntity>().Count();

				tx.Commit();
			}

			// Assert
			Assert.Zero(notCommitedTransactionEntitiesCount);
		}
	}
}

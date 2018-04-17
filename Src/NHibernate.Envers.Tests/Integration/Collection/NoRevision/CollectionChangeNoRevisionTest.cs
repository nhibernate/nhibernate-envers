using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Integration.Collection.NoRevision
{
	public class CollectionChangeNoRevisionTest : AbstractCollectionChangeTest
	{
		public CollectionChangeNoRevisionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override bool RevisionOnCollectionChange
		{
			get { return false; }
		}

		protected override IEnumerable<long> ExpectedPersonRevisions
		{
			get { return new long[] {1}; }
		}
	}
}
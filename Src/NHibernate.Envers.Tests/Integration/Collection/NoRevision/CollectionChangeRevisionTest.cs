using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Integration.Collection.NoRevision
{
	public class CollectionChangeRevisionTest : AbstractCollectionChangeTest
	{
		public CollectionChangeRevisionTest(string strategyType) : base(strategyType)
		{
		}

		protected override bool RevisionOnCollectionChange
		{
			get { return true; }
		}

		protected override IEnumerable<long> ExpectedPersonRevisions
		{
			get { return new long[] { 1, 3 }; }
		}
	}
}
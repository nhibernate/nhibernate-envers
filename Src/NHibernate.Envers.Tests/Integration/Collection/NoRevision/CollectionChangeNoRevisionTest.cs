using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Collection.NoRevision
{
	[TestFixture]
	public class CollectionChangeNoRevisionTest : AbstractCollectionChangeTest
	{
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
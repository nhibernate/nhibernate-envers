using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Collection.NoRevision
{
	[TestFixture]
	public class CollectionChangeRevisionTest : AbstractCollectionChangeTest
	{
		protected override string RevisionOnCollectionChange
		{
			get { return "true"; }
		}

		protected override IEnumerable<long> ExpectedPersonRevisions
		{
			get { return new long[] { 1, 3 }; }
		}
	}
}
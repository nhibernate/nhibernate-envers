using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.PersistRevisionInfo
{
	public partial class FlushGetCurrentRevisionTest : TestBase
	{
		public FlushGetCurrentRevisionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			// Nothing to do
		}

		/// <summary>
		/// When saving many objects, you should flush and clear a session
		/// (see "13.1. Batch inserts" in NH reference). This test demonstates
		/// an incorrect value of GetCurrentRevision when flushing a session.
		/// </summary>
		/// <param name="flushAndClear"></param>
		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void CompareRevisionIdToAuditedRevisionWithOptionalFlush(bool flushAndClear)
		{
			int id;
			int revisionId;

			var x = new StrTestEntity { Str = "x" };
			using (var tx = Session.BeginTransaction())
			{
				id = (int)Session.Save(x);
				// Get current revision id (this should be equal to the revision of "x")
				revisionId = AuditReader().GetCurrentRevision<DefaultRevisionEntity>(true).Id;

				if (flushAndClear)
				{
					// Flush session
					Session.Flush();
					// Clear cache (this seems to cause the problem)
					Session.Clear();
				}

				tx.Commit();
			}

			var revisions = AuditReader().GetRevisions(typeof(StrTestEntity), id);
			Assert.That(revisions.Count(), Is.EqualTo(1));
			// If the following assertion fails, the "x" has a different revision
			Assert.That(revisions.Last(), Is.EqualTo(revisionId));
		}
	}
}
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	//rk - mapkeyattribute should (hopefully) go away!
	public partial class BidirectionalMapKeyTest : TestBase
	{
		private int ed_id;
		private int ing1_id;
		private int ing2_id;

		public BidirectionalMapKeyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed = new RefEdMapKeyEntity();
			var ing1 = new RefIngMapKeyEntity {Data = "a", Reference = ed};
			var ing2 = new RefIngMapKeyEntity {Data = "b"};
			
			// Revision 1 (intialy 1 relation: ing1 -> ed)
			using (var tx = Session.BeginTransaction())
			{
				ed_id = (int) Session.Save(ed);
				ing1_id = (int) Session.Save(ing1);
				ing2_id = (int) Session.Save(ing2);
				tx.Commit();
			}
			// Revision 2 (adding second relation: ing2 -> ed)
			using (var tx = Session.BeginTransaction())
			{
				ing2.Reference = ed;
				tx.Commit();
			}
		}


		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(RefEdMapKeyEntity), ed_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(RefIngMapKeyEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(RefIngMapKeyEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryOfEd()
		{
			var ing1 = Session.Get<RefIngMapKeyEntity>(ing1_id);
			var ing2 = Session.Get<RefIngMapKeyEntity>(ing2_id);

			var rev1 = AuditReader().Find<RefEdMapKeyEntity>(ed_id, 1);
			var rev2 = AuditReader().Find<RefEdMapKeyEntity>(ed_id, 2);

			CollectionAssert.AreEquivalent(new Dictionary<string, RefIngMapKeyEntity> { { "a", ing1 } }, rev1.IdMap);
			CollectionAssert.AreEquivalent(new Dictionary<string, RefIngMapKeyEntity> { { "a", ing1 }, { "b", ing2 } }, rev2.IdMap);
		}
	}
}
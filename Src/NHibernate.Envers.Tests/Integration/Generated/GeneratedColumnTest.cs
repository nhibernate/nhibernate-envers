using System.Linq;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Generated
{
	public partial class GeneratedColumnTest : TestBase
	{
		private int entityId;
		
		public GeneratedColumnTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			SimpleEntity se = new SimpleEntity();
			se.Data = "data";
			
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(se);
				tx.Commit();
				Session.Clear();
			}

			entityId = se.Id;

			// Revision 2
			using (var tx = Session.BeginTransaction())
			{
				se = Session.Get<SimpleEntity>(entityId);
				se.Data = "data2";
				Session.Merge(se);
				tx.Commit();
			}

			// Revision 3
			using (var tx = Session.BeginTransaction())
			{
				se = Session.Get<SimpleEntity>(entityId);
				Session.Delete(se);
				tx.Commit();
			}
		}

		[Test]
		public void GetRevisionCounts()
		{
			Assert.AreEqual(3, AuditReader().GetRevisions(typeof(SimpleEntity), entityId).Count());
		}

		[Test]
		public void TestRevisionHistory()
		{
			// revision - insertion
			SimpleEntity rev1 = AuditReader().Find<SimpleEntity>(entityId, 1);
			Assert.AreEqual("data", rev1.Data);
			Assert.AreEqual(1, rev1.CaseNumberInsert);

			// revision - update
			SimpleEntity rev2 = AuditReader().Find<SimpleEntity>(entityId, 2);
			Assert.AreEqual("data2", rev2.Data);
			Assert.AreEqual(1, rev2.CaseNumberInsert);

			// revision - deletion
			SimpleEntity rev3 = AuditReader().Find<SimpleEntity>(entityId, 3);
			Assert.AreEqual("data2", rev2.Data);
			Assert.AreEqual(1, rev2.CaseNumberInsert);
		}
	}
}
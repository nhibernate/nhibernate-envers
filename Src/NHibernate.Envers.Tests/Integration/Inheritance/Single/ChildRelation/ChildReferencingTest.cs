using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Single.ChildRelation
{
	public partial class ChildReferencingTest : TestBase
	{
		private int re_id1;
		private int re_id2;
		private int c_id;

		public ChildReferencingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			re_id1 = 1;
			re_id2 = 10;
			c_id = 100;

			var re1 = new ReferencedToChildEntity { Id = re_id1 };
			var re2 = new ReferencedToChildEntity { Id = re_id2 };
			var cie = new ChildIngEntity { Id = c_id, Data = "y", Number = 1 };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(re1);
				Session.Save(re2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				cie.Referenced = re1;
				Session.Save(cie);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				cie.Referenced = re2;
				tx.Commit();
			}
		}


		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(ReferencedToChildEntity), re_id1));
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(ReferencedToChildEntity), re_id2));
			CollectionAssert.AreEquivalent(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(ChildIngEntity), c_id));
		}

		[Test]
		public void VerifyHistoryOfReferencedCollection1()
		{
			CollectionAssert.IsEmpty(AuditReader().Find<ReferencedToChildEntity>(re_id1, 1).Referencing);
			CollectionAssert.AreEquivalent(new[] { new ChildIngEntity { Id = c_id, Data = "y", Number = 1 } }, AuditReader().Find<ReferencedToChildEntity>(re_id1, 2).Referencing);
			CollectionAssert.IsEmpty(AuditReader().Find<ReferencedToChildEntity>(re_id1, 3).Referencing);
		}

		[Test]
		public void VerifyHistoryOfReferencedCollection2()
		{
			CollectionAssert.IsEmpty(AuditReader().Find<ReferencedToChildEntity>(re_id2, 1).Referencing);
			CollectionAssert.IsEmpty(AuditReader().Find<ReferencedToChildEntity>(re_id2, 2).Referencing);
			CollectionAssert.AreEquivalent(new[] { new ChildIngEntity { Id = c_id, Data = "y", Number = 1 } }, AuditReader().Find<ReferencedToChildEntity>(re_id2, 3).Referencing);
		}

		[Test]
		public void VerifyChildHistory()
		{
			Assert.IsNull(AuditReader().Find<ChildIngEntity>(c_id, 1));
			Assert.AreEqual(new ReferencedToChildEntity { Id = re_id1 }, AuditReader().Find<ChildIngEntity>(c_id, 2).Referenced);
			Assert.AreEqual(new ReferencedToChildEntity { Id = re_id2 }, AuditReader().Find<ChildIngEntity>(c_id, 3).Referenced);
		}
	}
}
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.Relation.Unidirectional
{
	public partial class UnidirectionalDoubleAbstractTest : TestBase
	{
		private long cce1_id;
		private int cse1_id;

		public UnidirectionalDoubleAbstractTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var cce1 = new ContainedEntity();
			var cse1 = new SetEntity();
			cse1.Entities.Add(cce1);

			using (var tx = Session.BeginTransaction())
			{
				cce1_id = (long) Session.Save(cce1);
				cse1_id = (int) Session.Save(cse1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(ContainedEntity), cce1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(SetEntity), cse1_id));
		}

		[Test]
		public void VerifyHistoryOfReferencedCollection()
		{
			var cce1 = Session.Get<ContainedEntity>(cce1_id);
			var entities = AuditReader().Find<SetEntity>(cse1_id, 1).Entities;
			Assert.AreEqual(1, entities.Count);
			CollectionAssert.Contains(entities, cce1);
		}
	}
}
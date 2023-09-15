using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Primitive
{
	public partial class PrimitiveAddDeleteTest : TestBase
	{
		private int id1;

		public PrimitiveAddDeleteTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var pte = new PrimitiveTestEntity { Number = 10, Number2 = 11 };

			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(pte);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				pte.Number = 20;
				pte.Number2 = 21;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(pte);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(PrimitiveTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new PrimitiveTestEntity { Id = id1, Number = 10, Number2 = 0 };
			var ver2 = new PrimitiveTestEntity { Id = id1, Number = 20, Number2 = 0 };

			Assert.AreEqual(ver1, AuditReader().Find<PrimitiveTestEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<PrimitiveTestEntity>(id1, 2));
			Assert.IsNull(AuditReader().Find<PrimitiveTestEntity>(id1, 3));
		}

		[Test]
		public void VerifyQueryWithDeleted()
		{
			var entities = AuditReader().CreateQuery()
							.ForRevisionsOfEntity(typeof(PrimitiveTestEntity), true, true).GetResultList();

			var expected = new[]
			               	{
			               		new PrimitiveTestEntity {Id = id1, Number = 10, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 20, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 0, Number2 = 0}
			               	};
			CollectionAssert.AreEqual(expected, entities);
		}

		[Test]
		public void VerifyQueryWithDeletedUsingGeneric()
		{
			var entities = AuditReader().CreateQuery().ForRevisionsOf<PrimitiveTestEntity>(true).Results();

			var expected = new[]
			               	{
			               		new PrimitiveTestEntity {Id = id1, Number = 10, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 20, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 0, Number2 = 0}
			               	};
			entities.Should().Have.SameSequenceAs(expected);
		}

		[Test]
		public void VerifyQueryWithNoDeletedUsingGeneric()
		{
			var entities = AuditReader().CreateQuery()
							.ForRevisionsOf<PrimitiveTestEntity>().Results();

			var expected = new[]
			               	{
			               		new PrimitiveTestEntity {Id = id1, Number = 10, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 20, Number2 = 0}
			               	};
			entities.Should().Have.SameSequenceAs(expected);
		}
	}
}
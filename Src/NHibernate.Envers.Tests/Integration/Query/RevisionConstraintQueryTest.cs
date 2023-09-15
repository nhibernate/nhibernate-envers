using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class RevisionConstraintQueryTest : TestBase
	{
		private int id1;

		public RevisionConstraintQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			var site1 = new StrIntTestEntity { Str = "a", Number = 10 };
			var site2 = new StrIntTestEntity { Str = "b", Number = 15 };

			using (var tx = Session.BeginTransaction())
			{
				id1=(int) Session.Save(site1);
				Session.Save(site2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				site1.Str = "d";
				site2.Number = 20;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				site1.Number = 1;
				site2.Str = "z";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				site1.Number = 5;
				site2.Str = "a";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionLtQuery()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber().Distinct())
						.Add(AuditEntity.RevisionNumber().Lt(3))
						.GetResultList();
			CollectionAssert.AreEquivalent(new[]{1, 2}, result);
		}

		[Test]
		public void VerifyRevisionGeQuery()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber().Distinct())
						.Add(AuditEntity.RevisionNumber().Ge(2))
						.GetResultList();
			CollectionAssert.AreEquivalent(new[] { 2, 3, 4 }, result);
		}

		[Test]
		public void VerifyRevisionLeWithPropertyQuery()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.RevisionNumber().Lt(3))
						.Add(AuditEntity.Property("Str").Eq("a"))
						.GetResultList();
			CollectionAssert.AreEquivalent(new[] { 1 }, result);
		}

		[Test]
		public void VerifyRevisionGtWithPropertyQuery()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.RevisionNumber().Gt(1))
						.Add(AuditEntity.Property("Number").Lt(10))
						.GetResultList();
			CollectionAssert.AreEquivalent(new[] { 3, 4 }, result);
		}

		[Test]
		public void VerifyRevisionProjectionQuery()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber().Max())
						.AddProjection(AuditEntity.RevisionNumber().Count())
						.AddProjection(AuditEntity.RevisionNumber().CountDistinct())
						.AddProjection(AuditEntity.RevisionNumber().Min())
						.Add(AuditEntity.Id().Eq(id1))
						.GetSingleResult<object[]>();
			Assert.AreEqual(4, result[0]);
			Assert.AreEqual(4, result[1]);
			Assert.AreEqual(4, result[2]);
			Assert.AreEqual(1, result[3]);
		}

		[Test]
		public void VerifyRevisionOrderQuery()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Id().Eq(id1))
						.AddOrder(AuditEntity.RevisionNumber().Desc())
						.GetResultList();
			CollectionAssert.AreEqual(new[]{4,3,2,1}, result);
		}

		[Test]
		public void VerifyRevisionCountQuery()
		{
			// The query shouldn't be ordered as always, otherwise - we get an exception.
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber().Count())
						.Add(AuditEntity.Id().Eq(id1))
						.GetSingleResult<long>();
			Assert.AreEqual(4, result);
		}

		[Test]
		public void VerifyRevisionTypeEqQuery()
		{
			// The query shouldn't be ordered as always, otherwise - we get an exception.
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), true, true)
						.Add(AuditEntity.Id().Eq(id1))
						.Add(AuditEntity.RevisionType().Eq(RevisionType.Modified))
						.GetResultList();
			CollectionAssert.AreEqual(
				new[]
					{
						new StrIntTestEntity {Str = "d", Number = 10, Id = id1}, 
						new StrIntTestEntity {Str = "d", Number = 1, Id = id1},
						new StrIntTestEntity {Str = "d", Number = 5, Id = id1}
					}, result);
		}

		[Test]
		public void VerifyRevisionTypeNeQuery()
		{
			// The query shouldn't be ordered as always, otherwise - we get an exception.
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), true, true)
						.Add(AuditEntity.Id().Eq(id1))
						.Add(AuditEntity.RevisionType().Ne(RevisionType.Modified))
						.GetResultList();
			CollectionAssert.AreEqual(
				new[]
					{
						new StrIntTestEntity {Str = "a", Number = 10, Id = id1}
					}, result);
		}
	}
}
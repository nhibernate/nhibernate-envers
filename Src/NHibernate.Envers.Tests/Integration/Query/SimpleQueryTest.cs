using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class SimpleQueryTest : TestBase
	{
		private int id1;
		private int id2;
		private int id3;
		private EmbId embId;

		public SimpleQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings =>
			new[] {"Entities.Mapping.hbm.xml", "Entities.Ids.Mapping.hbm.xml"};

		protected override void Initialize()
		{
			var site1 = new StrIntTestEntity { Str = "a", Number = 10 };
			var site2 = new StrIntTestEntity { Str = "a", Number = 10 };
			var site3 = new StrIntTestEntity { Str = "b", Number = 5 };
			embId = new EmbId{X = 3, Y = 4};
			
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(site1);
				id2 = (int) Session.Save(site2);
				id3 = (int) Session.Save(site3);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(new EmbIdTestEntity{Id = embId, Str1 = "something"});
				site1.Str = "aBc";
				site2.Number = 20;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				site3.Str = "a";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(site1);
				tx.Commit();
			}
		}

		[Test]
		public void EntitiesIdQuery()
		{
			var ver2 = AuditReader().CreateQuery()
						.ForEntitiesAtRevision(typeof (StrIntTestEntity), 2)
						.Add(AuditEntity.Id().Eq(id2))
						.GetSingleResult<StrIntTestEntity>();
			Assert.AreEqual(new StrIntTestEntity { Id = id2, Str = "a", Number = 20 }, ver2);
		}

		[Test]
		public void EntitiesPropertyEqualsQuery()
		{
			var ver1 = AuditReader().CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 1)
						.Add(AuditEntity.Property("Str").Eq("a"))
						.GetResultList();
			var ver2 = AuditReader().CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 2)
						.Add(AuditEntity.Property("Str").Eq("a"))
						.GetResultList();
			var ver3 = AuditReader().CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 3)
						.Add(AuditEntity.Property("Str").Eq("a"))
						.GetResultList();
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id1, Str = "a", Number = 10}, 
							new StrIntTestEntity {Id = id2, Str = "a", Number = 10}
						}, ver1);
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id2, Str = "a", Number = 20}
						}, ver2);
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id2, Str = "a", Number = 20},
							new StrIntTestEntity {Id = id3, Str = "a", Number = 5}
						}, ver3);
		}

		[Test]
		public void EntitiesPropertyLeQuery()
		{
			var ver1 = AuditReader().CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 1)
						.Add(AuditEntity.Property("Number").Le(10))
						.GetResultList();
			var ver2 = AuditReader().CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 2)
						.Add(AuditEntity.Property("Number").Le(10))
						.GetResultList();
			var ver3 = AuditReader().CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 3)
						.Add(AuditEntity.Property("Number").Le(10))
						.GetResultList();
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id1, Str = "a", Number = 10}, 
							new StrIntTestEntity {Id = id2, Str = "a", Number = 10},
							new StrIntTestEntity {Id = id3, Str = "b", Number = 5}
						}, ver1);
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id1, Str = "aBc", Number = 10},
							new StrIntTestEntity {Id = id3, Str = "b", Number = 5}
						}, ver2);
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id1, Str = "aBc", Number = 10},
							new StrIntTestEntity {Id = id3, Str = "a", Number = 5}
						}, ver3);
		}

		[Test]
		public void RevisionsPropertyEqQuery()
		{
			var revs_id1 = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Property("Str").Le("a"))
						.Add(AuditEntity.Id().Eq(id1))
						.GetResultList();
			var revs_id2 = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Property("Str").Le("a"))
						.Add(AuditEntity.Id().Eq(id2))
						.GetResultList();
			var revs_id3 = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Property("Str").Le("a"))
						.Add(AuditEntity.Id().Eq(id3))
						.GetResultList();
			CollectionAssert.AreEquivalent(new[] { 1 }, revs_id1);
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, revs_id2);
			CollectionAssert.AreEquivalent(new[] { 3 }, revs_id3);
		}

		[Test]
		public void RevisionsPropertyEqQueryForHistory()
		{
			var revs_id1 = AuditReader().CreateQuery().ForHistoryOf<StrIntTestEntity, DefaultRevisionEntity>()
						.Add(AuditEntity.Property("Str").Le("a"))
						.Add(AuditEntity.Id().Eq(id1))
						.Results().Select(x=> x.RevisionEntity.Id);
			var revs_id2 = AuditReader().CreateQuery().ForHistoryOf<StrIntTestEntity, DefaultRevisionEntity>()
						.Add(AuditEntity.Property("Str").Le("a"))
						.Add(AuditEntity.Id().Eq(id2))
						.Results().Select(x => x.RevisionEntity.Id);
			var revs_id3 = AuditReader().CreateQuery().ForHistoryOf<StrIntTestEntity, DefaultRevisionEntity>()
						.Add(AuditEntity.Property("Str").Le("a"))
						.Add(AuditEntity.Id().Eq(id3))
						.Results().Select(x => x.RevisionEntity.Id);
			CollectionAssert.AreEquivalent(new[] { 1 }, revs_id1);
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, revs_id2);
			CollectionAssert.AreEquivalent(new[] { 3 }, revs_id3);
		}

		[Test]
		public void SelectEntitiesQuery()
		{
			var result = AuditReader().CreateQuery()
							.ForRevisionsOfEntity(typeof (StrIntTestEntity), true, false)
							.Add(AuditEntity.Id().Eq(id1))
							.GetResultList<StrIntTestEntity>();
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id1, Str = "a", Number = 10},
							new StrIntTestEntity {Id = id1, Str = "aBc", Number = 10}
						}, result);
		}

		[Test]
		public void SelectEntitiesAndRevisionsQuery()
		{
			var result = AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.Add(AuditEntity.Id().Eq(id1))
				.GetResultList<IList>();

			Assert.AreEqual(new StrIntTestEntity { Id = id1, Str = "a", Number = 10 }, result[0][0]);
			Assert.AreEqual(new StrIntTestEntity { Id = id1, Str = "aBc", Number = 10 }, result[1][0]);
			Assert.AreEqual(new StrIntTestEntity { Id = id1 }, result[2][0]);

			Assert.AreEqual(1, ((DefaultRevisionEntity)result[0][1]).Id);
			Assert.AreEqual(2, ((DefaultRevisionEntity)result[1][1]).Id);
			Assert.AreEqual(4, ((DefaultRevisionEntity)result[2][1]).Id);

			Assert.AreEqual(RevisionType.Added, result[0][2]);
			Assert.AreEqual(RevisionType.Modified, result[1][2]);
			Assert.AreEqual(RevisionType.Deleted, result[2][2]);
		}

		[Test]
		public void SelectRevisionTypeQuery()
		{
			var result = AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.AddProjection(AuditEntity.RevisionType())
				.Add(AuditEntity.Id().Eq(id1))
				.GetResultList<RevisionType>();

			Assert.AreEqual(RevisionType.Added, result[0]);
			Assert.AreEqual(RevisionType.Modified, result[1]);
			Assert.AreEqual(RevisionType.Deleted, result[2]);
		}

		[Test]
		public void SelectRevisionTypeQueryUsingRevisionInfo()
		{
			var result = AuditReader().CreateQuery().ForHistoryOf<StrIntTestEntity, DefaultRevisionEntity>()
				.Add(AuditEntity.Id().Eq(id1))
				.Results();
			result.Select(x => x.Operation).Should().Have.SameSequenceAs(RevisionType.Added, RevisionType.Modified, RevisionType.Deleted);
		}

		[Test]
		public void EmptyRevisionOfEntityQuery()
		{
			var result = AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
				.GetResultList();
			Assert.AreEqual(7, result.Count);
		}

		[Test]
		public void EmptyConjunctionRevisionOfEntityQuery()
		{
			var result = AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.Add(AuditEntity.Conjunction())
				.GetResultList();
			Assert.AreEqual(7, result.Count);
		}

		[Test]
		public void EmptyDisjunctionRevisionOfEntityQuery()
		{
			var result = AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.Add(AuditEntity.Disjunction())
				.GetResultList();
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void ShouldFindEntitiesAddedAtRevision()
		{
			var result = AuditReader().CreateQuery()
				.ForEntitiesModifiedAtRevision(typeof (StrIntTestEntity).FullName, 1)
				.GetResultList<StrIntTestEntity>();
			var revisionType = AuditReader().CreateQuery()
				.ForEntitiesModifiedAtRevision(typeof(StrIntTestEntity), 1)
				.AddProjection(AuditEntity.RevisionType())
				.Add(AuditEntity.Id().Eq(id1))
				.GetSingleResult<RevisionType>();

			result.Should().Have.SameValuesAs(new StrIntTestEntity {Id = id1, Str = "a", Number = 10},
			                                    new StrIntTestEntity {Id = id2, Str = "a", Number = 10},
			                                    new StrIntTestEntity {Id = id3, Str = "b", Number = 5});

			revisionType.Should().Be.EqualTo(RevisionType.Added);
		}

		[Test]
		public void ShouldFindEntitiesModifiedAtRevision()
		{
			var result = AuditReader().CreateQuery()
				.ForEntitiesModifiedAtRevision(typeof(StrIntTestEntity), 2)
				.GetResultList<StrIntTestEntity>();
			var revisionType = AuditReader().CreateQuery()
				.ForEntitiesModifiedAtRevision(typeof(StrIntTestEntity), 2)
				.AddProjection(AuditEntity.RevisionType())
				.Add(AuditEntity.Id().Eq(id1))
				.GetSingleResult<RevisionType>();

			result.Should().Have.SameValuesAs(new StrIntTestEntity { Id = id1, Str = "aBc", Number = 10 },
															new StrIntTestEntity { Id = id2, Str = "a", Number = 20 });

			revisionType.Should().Be.EqualTo(RevisionType.Modified);
		}

		[Test]
		public void ShouldFindEntitiesRemovedAtRevision()
		{
			var result = AuditReader().CreateQuery()
				.ForEntitiesModifiedAtRevision(typeof(StrIntTestEntity).FullName, 4)
				.GetResultList<StrIntTestEntity>();
			var revisionType = AuditReader().CreateQuery()
				.ForEntitiesModifiedAtRevision(typeof(StrIntTestEntity), 4)
				.AddProjection(AuditEntity.RevisionType())
				.Add(AuditEntity.Id().Eq(id1))
				.GetSingleResult<RevisionType>();

			result.Should().Have.SameValuesAs(new StrIntTestEntity { Id = id1 });

			revisionType.Should().Be.EqualTo(RevisionType.Deleted);
		}

		[Test]
		public void VerifyEntityNotModifiedAtRevision()
		{
			var result = AuditReader().CreateQuery()
						.ForEntitiesModifiedAtRevision(typeof(StrIntTestEntity), 3)
						.Add(AuditEntity.Id().Eq(id1))
						.GetResultList<StrIntTestEntity>();
			result.Should().Be.Empty();
		}

		[Test]
		public void VerifyNoEntitiesModifiedAtRevision()
		{
			var result = AuditReader().CreateQuery()
						.ForEntitiesModifiedAtRevision(typeof(StrIntTestEntity).FullName, 5)
						.GetResultList<StrIntTestEntity>();
			result.Should().Be.Empty();
		}

		[Test]
		public void VerifyBetweenInsideDisjunction()
		{
			var result = AuditReader().CreateQuery()
				.ForRevisionsOf<StrIntTestEntity>()
				.Add(AuditEntity.Disjunction()
				     	.Add(AuditEntity.Property("Number").Between(0, 5))
				     	.Add(AuditEntity.Property("Number").Between(20, 100)))
				.Results();
			foreach (var number in result.Select(entity => entity.Number))
			{
				Assert.That(number, Is.InRange(0, 5).Or.InRange(20, 100));
			}
		}

		[Test]
		public void VerifyInsensitiveLike()
		{
			var site1 = new StrIntTestEntity {Id = id1, Number = 10, Str = "aBc"};
			AuditReader().CreateQuery()
			             .ForRevisionsOf<StrIntTestEntity>(false)
			             .Add(AuditEntity.Property("Str").InsensitiveLike("abc"))
			             .Single()
			             .Should().Be.EqualTo(site1);
		}

		[Test]
		public void VerifyInsensitiveLikeWithMatchMode()
		{
			var site1 = new StrIntTestEntity { Id = id1, Number = 10, Str = "aBc" };
			AuditReader().CreateQuery()
									 .ForRevisionsOf<StrIntTestEntity>(false)
									 .Add(AuditEntity.Property("Str").InsensitiveLike("BC", MatchMode.Anywhere))
									 .Single()
									 .Should().Be.EqualTo(site1);
		}

		[Test]
		public void VerifyIdPropertyRestriction()
		{
			var ver2 = AuditReader().CreateQuery()
				.ForEntitiesAtRevision<StrIntTestEntity>(2)
				.Add(AuditEntity.Property("Id").Eq(id2))
				.Single();
			ver2.Number.Should().Be.EqualTo(20);
			ver2.Str.Should().Be.EqualTo("a");
		}

		[Test]
		public void VerifyEmbeddedIdPropertyRestriction()
		{
			var ver2 = AuditReader().CreateQuery()
				.ForEntitiesAtRevision<EmbIdTestEntity>(2)
				.Add(AuditEntity.Property("Id.X").Eq(embId.X))
				.Add(AuditEntity.Property("Id.Y").Eq(embId.Y))
				.Single();
			ver2.Str1.Should().Be.EqualTo("something");
		}
	}
}
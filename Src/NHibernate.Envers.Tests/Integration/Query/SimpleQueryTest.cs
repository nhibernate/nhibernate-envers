using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query
{
	[TestFixture]
	public class SimpleQueryTest : TestBase
	{
		private int id1;
		private int id2;
		private int id3;

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
			var site2 = new StrIntTestEntity { Str = "a", Number = 10 };
			var site3 = new StrIntTestEntity { Str = "b", Number = 5 };

			using (var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(site1);
				id2 = (int) Session.Save(site2);
				id3 = (int) Session.Save(site3);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				site1.Str = "c";
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
			var ver2 = AuditReader.CreateQuery()
						.ForEntitiesAtRevision(typeof (StrIntTestEntity), 2)
						.Add(AuditEntity.Id().Eq(id2))
						.GetSingleResult<StrIntTestEntity>();
			Assert.AreEqual(new StrIntTestEntity { Id = id2, Str = "a", Number = 20 }, ver2);
		}

		[Test]
		public void EntitiesPropertyEqualsQuery()
		{
			var ver1 = AuditReader.CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 1)
						.Add(AuditEntity.Property("Str").Eq("a"))
						.GetResultList();
			var ver2 = AuditReader.CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 2)
						.Add(AuditEntity.Property("Str").Eq("a"))
						.GetResultList();
			var ver3 = AuditReader.CreateQuery()
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
			var ver1 = AuditReader.CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 1)
						.Add(AuditEntity.Property("Number").Le(10))
						.GetResultList();
			var ver2 = AuditReader.CreateQuery()
						.ForEntitiesAtRevision(typeof(StrIntTestEntity), 2)
						.Add(AuditEntity.Property("Number").Le(10))
						.GetResultList();
			var ver3 = AuditReader.CreateQuery()
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
							new StrIntTestEntity {Id = id1, Str = "c", Number = 10},
							new StrIntTestEntity {Id = id3, Str = "b", Number = 5}
						}, ver2);
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id1, Str = "c", Number = 10},
							new StrIntTestEntity {Id = id3, Str = "a", Number = 5}
						}, ver3);
		}

		[Test]
		public void RevisionsPropertyEqQuery()
		{
			var revs_id1 = AuditReader.CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Property("Str").Le("a"))
						.Add(AuditEntity.Id().Eq(id1))
						.GetResultList();
			var revs_id2 = AuditReader.CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Property("Str").Le("a"))
						.Add(AuditEntity.Id().Eq(id2))
						.GetResultList();
			var revs_id3 = AuditReader.CreateQuery()
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
		public void SelectEntitiesQuery()
		{
			var result = AuditReader.CreateQuery()
							.ForRevisionsOfEntity(typeof (StrIntTestEntity), true, false)
							.Add(AuditEntity.Id().Eq(id1))
							.GetResultList<StrIntTestEntity>();
			CollectionAssert.AreEquivalent(new[]
						{
							new StrIntTestEntity {Id = id1, Str = "a", Number = 10},
							new StrIntTestEntity {Id = id1, Str = "c", Number = 10}
						}, result);
		}

		[Test]
		public void SelectEntitiesAndRevisionsQuery()
		{
			var result = AuditReader.CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.Add(AuditEntity.Id().Eq(id1))
				.GetResultList<IList>();

			Assert.AreEqual(new StrIntTestEntity { Id = id1, Str = "a", Number = 10 }, result[0][0]);
			Assert.AreEqual(new StrIntTestEntity { Id = id1, Str = "c", Number = 10 }, result[1][0]);
			Assert.AreEqual(new StrIntTestEntity { Id = id1 }, result[2][0]);

			Assert.AreEqual(1, ((DefaultRevisionEntity)result[0][1]).Id);
			Assert.AreEqual(2, ((DefaultRevisionEntity)result[1][1]).Id);
			Assert.AreEqual(4, ((DefaultRevisionEntity)result[2][1]).Id);

			Assert.AreEqual(RevisionType.ADD, result[0][2]);
			Assert.AreEqual(RevisionType.MOD, result[1][2]);
			Assert.AreEqual(RevisionType.DEL, result[2][2]);
		}

		[Test]
		public void SelectRevisionTypeQuery()
		{
			var result = AuditReader.CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.AddProjection(AuditEntity.RevisionType())
				.Add(AuditEntity.Id().Eq(id1))
				.GetResultList<RevisionType>();

			Assert.AreEqual(RevisionType.ADD, result[0]);
			Assert.AreEqual(RevisionType.MOD, result[1]);
			Assert.AreEqual(RevisionType.DEL, result[2]);
		}

		[Test]
		public void EmptyRevisionOfEntityQuery()
		{
			var result = AuditReader.CreateQuery()
				.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
				.GetResultList();
			Assert.AreEqual(7, result.Count);
		}

		[Test]
		public void EmptyConjunctionRevisionOfEntityQuery()
		{
			var result = AuditReader.CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.Add(AuditEntity.Conjunction())
				.GetResultList();
			Assert.AreEqual(7, result.Count);
		}

		[Test]
		public void EmptyDisjunctionRevisionOfEntityQuery()
		{
			var result = AuditReader.CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.Add(AuditEntity.Disjunction())
				.GetResultList();
			Assert.AreEqual(0, result.Count);
		}
	}
}
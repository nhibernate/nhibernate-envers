using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class MaximalizePropertyQueryTest : TestBase
	{
		private int id1;
		private int id2;
		private int id3;
		private int id4;

		public MaximalizePropertyQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
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
			var site3 = new StrIntTestEntity { Str = "c", Number = 42 };
			var site4 = new StrIntTestEntity { Str = "d", Number = 52 };
			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(site1);
				id2 = (int) Session.Save(site2);
				id3 = (int) Session.Save(site3);
				id4 = (int) Session.Save(site4);
				tx.Commit();
			}
			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				site1.Str = "d";
				site2.Number = 20;
				tx.Commit();
			}
			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				site1.Number = 30;
				site2.Str = "z";
				tx.Commit();
			}
			//rev 4
			using (var tx = Session.BeginTransaction())
			{
				site1.Number = 5;
				site2.Str = "a";
				tx.Commit();
			}
			//rev 5
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(site4);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyMaximzeWithIdEq()
		{
			var revsId1 = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Property("Number").Maximize()
							.Add(AuditEntity.Id().Eq(id2)))
						.GetResultList();
			CollectionAssert.AreEqual(new[]{2, 3, 4}, revsId1);
		}

		[Test]
		public void VerifyMinimizeWithPropertyEq()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Property("Number").Minimize()
							.Add(AuditEntity.Property("Str").Eq("a")))
						.GetResultList();
			CollectionAssert.AreEqual(new[] { 1 }, result);
		}

		[Test]
		public void VerifyMaximizeRevision()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.RevisionNumber().Maximize()
							.Add(AuditEntity.Property("Number").Eq(10)))
						.GetResultList();
			CollectionAssert.AreEquivalent(new[]{2}, result);
		}

		[Test]
		public void VerifyMaximizeInDisjunction()
		{
			var idsToQuery = new[] {id1, id3};
			var disjuction = AuditEntity.Disjunction();
			foreach (var id in idsToQuery)
			{
				disjuction.Add(AuditEntity.RevisionNumber().Maximize().Add(AuditEntity.Id().Eq(id)));
			}
			var result = AuditReader().CreateQuery()
				.ForRevisionsOf<StrIntTestEntity>(true)
				.Add(disjuction)
				.Results();
			var idsSeen = new HashSet<int>();
			foreach (var entity in result)
			{
				var id = entity.Id;
				idsToQuery.Should().Contain(id);
				idsSeen.Add(id).Should().Be.True();
			}
		}

		[Test]
		public void VerifyAllLatestRevisionOfEntityType()
		{
			var result =AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
				.Add(AuditEntity.RevisionNumber().Maximize().ComputeAggregationInInstanceContext())
				.AddOrder(AuditEntity.Property("id" ).Asc())
				.GetResultList();

			result.Count.Should().Be.EqualTo(4);

			var result1 = (object[])result[0];
			var result2 = (object[])result[1];
			var result3 = (object[])result[2];
			var result4 = (object[])result[3];
			checkRevisionData(result1, 4, RevisionType.Modified, new StrIntTestEntity{Id=id1, Number = 5, Str = "d"});
			checkRevisionData(result2, 4, RevisionType.Modified, new StrIntTestEntity{Id=id2, Number = 20, Str = "a"});
			checkRevisionData(result3, 1, RevisionType.Added, new StrIntTestEntity{Id=id3, Number = 42, Str = "c"});
			checkRevisionData(result4, 5, RevisionType.Deleted, new StrIntTestEntity{Id=id4, Number = 0, Str = null});
		}

		private void checkRevisionData(IList<object> result, int revision, RevisionType type, StrIntTestEntity entity)
		{
			result[0].Should().Be.EqualTo(entity);
			((DefaultRevisionEntity) result[1]).Id.Should().Be.EqualTo(revision);
			result[2].Should().Be.EqualTo(type);
		}
	}
}
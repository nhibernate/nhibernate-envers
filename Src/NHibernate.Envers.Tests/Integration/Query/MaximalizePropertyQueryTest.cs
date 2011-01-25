using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query
{
	[TestFixture]
	public class MaximalizePropertyQueryTest : TestBase
	{
		private int id1;
		private int id2;

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
				id1 = (int) Session.Save(site1);
				id2 = (int) Session.Save(site2);
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
				site1.Number = 30;
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
		public void VerifyMaximzeWithEqId()
		{
			var revs_id1 = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
						.AddProjection(AuditEntity.RevisionNumber())
						.Add(AuditEntity.Property("Number").Maximize()
							.Add(AuditEntity.Id().Eq(id2)))
						.GetResultList();
			CollectionAssert.AreEqual(new[]{2, 3, 4}, revs_id1);
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
	}
}
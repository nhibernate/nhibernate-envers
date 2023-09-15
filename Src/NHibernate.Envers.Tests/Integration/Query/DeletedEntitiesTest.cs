using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class DeletedEntitiesTest : TestBase
	{
		private int id2;

		public DeletedEntitiesTest(AuditStrategyForTest strategyType) : base(strategyType)
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
			var site1 = new StrIntTestEntity {Str = "a", Number = 10};
			var site2 = new StrIntTestEntity {Str = "b", Number = 11};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(site1);
				id2 = (int) Session.Save(site2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(site2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyProjectionsInEntitiesAtRevision()
		{
			Assert.AreEqual(2, AuditReader().CreateQuery().ForEntitiesAtRevision(typeof (StrIntTestEntity), 1).GetResultList().Count);
			Assert.AreEqual(1, AuditReader().CreateQuery().ForEntitiesAtRevision(typeof (StrIntTestEntity), 2).GetResultList().Count);

			Assert.AreEqual(2, 
				AuditReader().CreateQuery()
					.ForEntitiesAtRevision(typeof(StrIntTestEntity), 1).AddProjection(AuditEntity.Id().Count()).GetResultList()[0]);
			Assert.AreEqual(1,
				AuditReader().CreateQuery()
					.ForEntitiesAtRevision(typeof(StrIntTestEntity), 2).AddProjection(AuditEntity.Id().Count()).GetResultList()[0]);
		}

		[Test]
		public void VerifyRevisionOfEntityWithoutDelete()
		{
			var result = AuditReader().CreateQuery()
					.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, false)
					.Add(AuditEntity.Id().Eq(id2))
					.GetResultList();

			Assert.AreEqual(1, result.Count);
			var res = (object[]) result[0];
			Assert.AreEqual(new StrIntTestEntity { Str = "b", Number = 11, Id=id2 }, res[0]);
			Assert.AreEqual(1, ((DefaultRevisionEntity)res[1]).Id);
			Assert.AreEqual(RevisionType.Added, res[2]);
		}
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Cache
{
	public partial class QueryCacheTest : TestBase
	{
		private int id1;

		public QueryCacheTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var ite = new IntTestEntity { Number = 10 };

			using(var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(ite);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				ite.Number = 20;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyCacheFindAfterRevisionOfEntityQuery()
		{
			var entsFromQuery = AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof (IntTestEntity), true, false)
				.GetResultList();

			var entFromFindRev1 = AuditReader().Find<IntTestEntity>(id1, 1);
			var entFromFindRev2 = AuditReader().Find<IntTestEntity>(id1, 2);

			Assert.AreSame(entFromFindRev1, entsFromQuery[0]);
			Assert.AreSame(entFromFindRev2, entsFromQuery[1]);
		}

		[Test]
		public void VerifyCacheFindAfterEntitiesAtRevisionQuery()
		{
			var entFromQuery = (IntTestEntity) AuditReader().CreateQuery()
			                                   	.ForEntitiesAtRevision(typeof (IntTestEntity), 1)
			                                   	.GetSingleResult();
			var entFromFind = AuditReader().Find<IntTestEntity>(id1, 1);
			Assert.AreSame(entFromFind, entFromQuery);
		}
	}
}
using System;
using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class AggregateQueryTest : TestBase
	{
		public AggregateQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ite1 = new IntTestEntity { Number = 2 };
			var ite2 = new IntTestEntity { Number = 10 };
			var ite3 = new IntTestEntity { Number = 8 };
			var uine1 = new UnusualIdNamingEntity {UniqueField = "Id1", VariousData = "data1"};

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ite1);
				Session.Save(ite2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ite3);
				Session.Save(uine1);
				ite1.Number = 0;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ite2.Number = 52;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyEntitiesAvgMaxQuery()
		{
			var ver1 = (object[])AuditReader().CreateQuery()
					.ForEntitiesAtRevision(typeof(IntTestEntity), 1)
					.AddProjection(AuditEntity.Property("Number").Max())
					.AddProjection(AuditEntity.Property("Number").Function("avg"))
					.GetSingleResult();
			var ver2 = (object[])AuditReader().CreateQuery()
					.ForEntitiesAtRevision(typeof(IntTestEntity), 2)
					.AddProjection(AuditEntity.Property("Number").Max())
					.AddProjection(AuditEntity.Property("Number").Function("avg"))
					.GetSingleResult();
			var ver3 = (object[])AuditReader().CreateQuery()
					.ForEntitiesAtRevision(typeof(IntTestEntity), 3)
					.AddProjection(AuditEntity.Property("Number").Max())
					.AddProjection(AuditEntity.Property("Number").Function("avg"))
					.GetSingleResult();

			Assert.AreEqual(10, ver1[0]);
			Assert.AreEqual(6, ver1[1]);

			Assert.AreEqual(10, ver2[0]);
			Assert.AreEqual(6, ver2[1]);

			Assert.AreEqual(52, ver3[0]);
			Assert.AreEqual(20, ver3[1]);
		}

		[Test]
		public void VerifyEntityIdProjection()
		{
			var maxId = AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (IntTestEntity), true, true)
			                         .AddProjection(AuditEntity.Id().Max())
			                         .Add(AuditEntity.RevisionNumber().Gt(2))
			                         .GetSingleResult();
			maxId.Should().Be.EqualTo(2);
		}

		[Test]
		public void VerifyEntityIdRestriction()
		{
			AuditReader().CreateQuery().ForRevisionsOf<IntTestEntity>(true)
			    .Add(AuditEntity.Id().Between(2, 3))
			    .Results()
					.Should().Have.SameValuesAs(new IntTestEntity {Id = 2, Number = 10}, 
												new IntTestEntity {Id = 3, Number = 8},
												new IntTestEntity {Id = 2, Number = 52});
		}

		[Test]
		public void VerifyEntityIdOrdering()
		{
			AuditReader().CreateQuery().ForRevisionsOf<IntTestEntity>(true)
			             .Add(AuditEntity.RevisionNumber().Lt(2))
			             .AddOrder(AuditEntity.Id().Desc())
			             .Results()
			             .Should().Have.SameSequenceAs(new IntTestEntity {Id = 2, Number = 10}, new IntTestEntity {Id = 1, Number = 2});
		}

		[Test]
		public void VerifyUnusualIdFieldName()
		{
			AuditReader().CreateQuery().ForRevisionsOf<UnusualIdNamingEntity>(true)
			             .Add(AuditEntity.Id().Like("Id1"))
			             .Single()
			             .Should().Be.EqualTo(new UnusualIdNamingEntity {UniqueField = "Id1", VariousData = "data1"});
		}

		[Test]
		public void VerifyEntityIdModifiedFlagNotSupported()
		{
			try
			{
				AuditReader().CreateQuery().ForRevisionsOf<IntTestEntity>(true)
				             .Add(AuditEntity.Id().HasChanged())
				             .Results();
			}
			catch (NotSupportedException)
			{
				try
				{
					AuditReader().CreateQuery().ForRevisionsOf<IntTestEntity>(true)
											 .Add(AuditEntity.Id().HasNotChanged())
											 .Results();
				}
				catch (NotSupportedException)
				{
					return;
				}
				Assert.Fail();
			}
			Assert.Fail();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.Ids.Mapping.hbm.xml" };
			}
		}
	}
}
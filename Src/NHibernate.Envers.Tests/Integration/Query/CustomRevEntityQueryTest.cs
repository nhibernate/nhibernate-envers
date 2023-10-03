using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.RevEntity;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class CustomRevEntityQueryTest : TestBase
	{
		private int id1;
		private int id2;
		private long timestamp;

		public CustomRevEntityQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.CustomRevEntity.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			var site1 = new StrIntTestEntity {Str = "a", Number = 10};
			var site2 = new StrIntTestEntity {Str = "b", Number = 15};

			using(var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(site1);
				id2 = (int) Session.Save(site2);
				tx.Commit();
			}
			Thread.Sleep(100);
			timestamp = DateTime.UtcNow.Ticks;
			Thread.Sleep(100);
			using(var tx = Session.BeginTransaction())
			{
				site1.Str = "c";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsOfId1Query()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
						.Add(AuditEntity.Id().Eq(id1))
						.GetResultList();

			Assert.AreEqual(new StrIntTestEntity {Str = "a", Number = 10, Id = id1}, ((IList)result[0])[0]);
			var customRevEntity1 = ((IList) result[0])[1];
			Assert.IsInstanceOf<CustomRevEntity>(customRevEntity1);
			Assert.AreEqual(1, ((CustomRevEntity)customRevEntity1).CustomId);

			Assert.AreEqual(new StrIntTestEntity { Str = "c", Number = 10, Id = id1 }, ((IList)result[1])[0]);
			var customRevEntity2 = ((IList)result[1])[1];
			Assert.IsInstanceOf<CustomRevEntity>(customRevEntity2);
			Assert.AreEqual(2, ((CustomRevEntity)customRevEntity2).CustomId);
		}

		[Test]
		public void VerifyRevisionsOfId2Query()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof(StrIntTestEntity), false, true)
						.Add(AuditEntity.Id().Eq(id2))
						.GetResultList();

			Assert.AreEqual(new StrIntTestEntity { Str = "b", Number = 15, Id = id2 }, ((IList)result[0])[0]);
			var customRevEntity1 = ((IList)result[0])[1];
			Assert.IsInstanceOf<CustomRevEntity>(customRevEntity1);
			Assert.AreEqual(1, ((CustomRevEntity)customRevEntity1).CustomId);
		}

		[Test]
		public void VerifyRevisionPropertyRestriction()
		{
			var result = AuditReader().CreateQuery()
						.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
						.Add(AuditEntity.Id().Eq(id1))
						.Add(AuditEntity.RevisionProperty("CustomTimestamp").Ge(timestamp))
						.GetResultList();
			Assert.AreEqual(new StrIntTestEntity { Str = "c", Number = 10, Id = id1 }, ((IList)result[0])[0]);
			var customRevEntity1 = ((IList)result[0])[1];
			Assert.IsInstanceOf<CustomRevEntity>(customRevEntity1);
			Assert.IsTrue(((CustomRevEntity)customRevEntity1).CustomTimestamp >= timestamp);
		}
	}
}
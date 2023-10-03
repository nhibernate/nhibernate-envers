using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class OrderByLimitQueryTest : TestBase
	{
		private int id1;
		private int id2;
		private int id3;
		private int id4;
		private int id5;

		public OrderByLimitQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
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
			var ite1 = new IntTestEntity { Number = 12 };
			var ite2 = new IntTestEntity { Number = 5 };
			var ite3 = new IntTestEntity { Number = 8 };
			var ite4 = new IntTestEntity { Number = 1 };
			var ite5 = new IntTestEntity { Number = 3 };

			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(ite1);
				id2 = (int)Session.Save(ite2);
				id3 = (int)Session.Save(ite3);
				id4 = (int)Session.Save(ite4);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				id5 = (int)Session.Save(ite5);
				ite1.Number = 0;
				ite4.Number = 15;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyEntitiesOrderLimitByQueryRev1()
		{
			var res_0_to_1 = AuditReader().CreateQuery()
							.ForEntitiesAtRevision(typeof(IntTestEntity), 1)
							.AddOrder(AuditEntity.Property("Number").Desc())
							.SetFirstResult(0)
							.SetMaxResults(2)
							.GetResultList();
			var res_2_to_3 = AuditReader().CreateQuery()
							.ForEntitiesAtRevision(typeof(IntTestEntity), 1)
							.AddOrder(AuditEntity.Property("Number").Desc())
							.SetFirstResult(2)
							.SetMaxResults(2)
							.GetResultList();
			var res_empty = AuditReader().CreateQuery()
							.ForEntitiesAtRevision(typeof(IntTestEntity), 1)
							.AddOrder(AuditEntity.Property("Number").Desc())
							.SetFirstResult(4)
							.SetMaxResults(2)
							.GetResultList();
			CollectionAssert.AreEqual(new[]
										{
			                          		new IntTestEntity {Id = id1, Number = 12}, new IntTestEntity {Id = id3, Number = 8}
			                          	}, res_0_to_1);
			CollectionAssert.AreEqual(new[]
										{
			                          		new IntTestEntity {Id = id2, Number = 5}, new IntTestEntity {Id = id4, Number = 1}
			                          	}, res_2_to_3);
			CollectionAssert.IsEmpty(res_empty);
		}

		[Test]
		public void VerifyEntitiesOrderLimitByQueryRev2()
		{
			var res_0_to_1 = AuditReader().CreateQuery()
							.ForEntitiesAtRevision(typeof(IntTestEntity), 2)
							.AddOrder(AuditEntity.Property("Number").Desc())
							.SetFirstResult(0)
							.SetMaxResults(2)
							.GetResultList();
			var res_2_to_3 = AuditReader().CreateQuery()
							.ForEntitiesAtRevision(typeof(IntTestEntity), 2)
							.AddOrder(AuditEntity.Property("Number").Desc())
							.SetFirstResult(2)
							.SetMaxResults(2)
							.GetResultList();
			var res_4 = AuditReader().CreateQuery()
							.ForEntitiesAtRevision(typeof(IntTestEntity), 2)
							.AddOrder(AuditEntity.Property("Number").Desc())
							.SetFirstResult(4)
							.SetMaxResults(2)
							.GetResultList();
			CollectionAssert.AreEqual(new[]
										{
			                          		new IntTestEntity {Id = id4, Number = 15}, new IntTestEntity {Id = id3, Number = 8}
			                          	}, res_0_to_1);
			CollectionAssert.AreEqual(new[]
										{
			                          		new IntTestEntity {Id = id2, Number = 5}, new IntTestEntity {Id = id5, Number = 3}
			                          	}, res_2_to_3);
			CollectionAssert.AreEqual(new[]
										{
			                          		new IntTestEntity {Id = id1, Number = 0}
			                          	}, res_4);
		}
	}
}
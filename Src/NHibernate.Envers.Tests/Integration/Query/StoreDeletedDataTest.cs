using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class StoreDeletedDataTest : TestBase
	{
		private int id1;

		public StoreDeletedDataTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml" };
			}
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
		}

		protected override void Initialize()
		{
			var site1 = new StrIntTestEntity { Str = "a", Number = 10 };
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(site1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(site1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsPropertyEqQuery()
		{
			var revsId1 = AuditReader().CreateQuery()
							.ForRevisionsOfEntity(typeof (StrIntTestEntity), false, true)
							.Add(AuditEntity.Id().Eq(id1))
							.GetResultList<IList>();
			Assert.AreEqual(2, revsId1.Count);
			Assert.AreEqual(new StrIntTestEntity { Str = "a", Number = 10, Id=id1 }, revsId1[0][0]);
			Assert.AreEqual(new StrIntTestEntity { Str = "a", Number = 10, Id = id1 }, revsId1[1][0]);
		}
	}
}
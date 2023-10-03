using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.NetSpecific.Integration.OneToOne;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Proxy
{
	public partial class OneToOneRemovedObjectQueryTest : TestBase
	{
		private const int id =125;

		public OneToOneRemovedObjectQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var owning = new OneToOneOwningEntity {Id=id};
			var owned = new OneToOneOwnedEntity { Data = "Demo Data 1", Owning = owning};
			
			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(owning);
				Session.Save(owned);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				owned.Data = "Demo Data 2";
				tx.Commit();
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(owned);
				Session.Delete(owning);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHistoryOfOwning()
		{
			AuditReader().Find<OneToOneOwningEntity>(id, 1)
									 .Owned.Data.Should().Be.EqualTo("Demo Data 1");
			AuditReader().Find<OneToOneOwningEntity>(id, 2)
									 .Owned.Data.Should().Be.EqualTo("Demo Data 2");
			AuditReader().CreateQuery().ForRevisionsOf<OneToOneOwningEntity>(true)
			             .Add(AuditEntity.Id().Eq(id))
			             .Add(AuditEntity.RevisionNumber().Eq(3))
									 .Results().First()
									 .Owned.Data.Should().Be.EqualTo("Demo Data 2");
		}

		[Test]
		public void VerifyHistoryOfOwned()
		{
			AuditReader().Find<OneToOneOwnedEntity>(id, 1)
									 .Owning.Should().Not.Be.Null();
			AuditReader().Find<OneToOneOwnedEntity>(id, 2)
									 .Owning.Should().Not.Be.Null();
			AuditReader().CreateQuery().ForRevisionsOf<OneToOneOwnedEntity>(true)
									 .Add(AuditEntity.Id().Eq(id))
									 .Add(AuditEntity.RevisionNumber().Eq(3))
									 .Results().First()
									 .Owning.Should().Not.Be.Null();
		}


		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"NetSpecific.Integration.OneToOne.Mapping.hbm.xml"}; }
		}
	}
}
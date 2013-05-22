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
	[Ignore("Not yet fixed - NHE-123")]
	public class OneToOneSingleRemovedObjectQueryTest : TestBase
	{
		private const int id =135;

		public OneToOneSingleRemovedObjectQueryTest(string strategyType)
			: base(strategyType)
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
				Session.Delete(owned);
				tx.Commit();
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(owning);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHistoryOfOwning()
		{
			AuditReader().Find<OneToOneOwningEntity>(id, 1)
									 .Owned.Should().Not.Be.Null();
			AuditReader().Find<OneToOneOwningEntity>(id, 2)
									 .Owned.Should().Be.Null();
			AuditReader().CreateQuery().ForRevisionsOf<OneToOneOwningEntity>(true)
				.Add(AuditEntity.Id().Eq(id))
				.Add(AuditEntity.RevisionNumber().Eq(3))
				.Results().First()
				.Owned.Should().Be.Null();
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
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Performance.EvictAfterTx
{
	[TestFixture]
	public abstract class EvictAuditDataAfterCommitTest : TestBase
	{
		protected override void Initialize()
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetProperty(ConfigurationKey.AuditStrategy, AuditStrategy.AssemblyQualifiedName);
		}

		protected abstract System.Type AuditStrategy { get; }

		[Test]
		public void VerifySessionCacheClear()
		{
			var auditEntityNames = new[] { "NHibernate.Envers.Tests.Entities.StrTestEntity_AUD" };
			var ste = new StrTestEntity { Str = "data" };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ste);
				tx.Commit();
			}
			checkEmptyAuditSessionCache(auditEntityNames);

			using (var tx = Session.BeginTransaction())
			{
				ste.Str = "changed";
				tx.Commit();
			}

			checkEmptyAuditSessionCache(auditEntityNames);
		}

		[Test]
		public void VerifySessionCacheCollectionClear()
		{
			var auditEntityNames = new[] {"NHibernate.Envers.Tests.Entities.OneToMany.SetRefEdEntity_AUD",
																		  "NHibernate.Envers.Tests.Entities.OneToMany.SetRefIngEntity_AUD"};

			var ed1 = new SetRefEdEntity { Id = 1, Data = "data_ed_1" };
			var ed2 = new SetRefEdEntity { Id = 2, Data = "data_ed_2" };
			var ing1 = new SetRefIngEntity { Id = 3, Data = "data_ing_1" };
			var ing2 = new SetRefIngEntity { Id = 4, Data = "data_ing_2" };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ing1);
				Session.Save(ing2);
				tx.Commit();
			}

			checkEmptyAuditSessionCache(auditEntityNames);

			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed1;
				ing2.Reference = ed1;
				tx.Commit();
			}

			checkEmptyAuditSessionCache(auditEntityNames);

			using (var tx = Session.BeginTransaction())
			{
				var reffering = new HashedSet<SetRefIngEntity> { ing1, ing2 };
				ed2.Reffering = reffering;
				tx.Commit();
			}
			checkEmptyAuditSessionCache(auditEntityNames);

			using (var tx = Session.BeginTransaction())
			{
				ed2.Reffering.Remove(ing1);
				tx.Commit();
			}
			checkEmptyAuditSessionCache(auditEntityNames);

			using (var tx = Session.BeginTransaction())
			{
				ed2.Reffering.First().Data = "mod_data_ing_2";
				tx.Commit();
			}
			checkEmptyAuditSessionCache(auditEntityNames);
		}

		private void checkEmptyAuditSessionCache(ICollection<string> entityNames)
		{
			var persistenceContext = ((ISessionImplementor)Session).PersistenceContext;
			foreach (var entityEntry in persistenceContext.EntityEntries.Values.Cast<EntityEntry>())
			{
				entityNames.Should().Not.Contain(entityEntry.EntityName);
				entityEntry.EntityName
					.Should().Not.Be.EqualTo(typeof(DefaultRevisionEntity).FullName);
			}
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml", "Entities.OneToMany.Mapping.hbm.xml" }; }
		}
	}
}
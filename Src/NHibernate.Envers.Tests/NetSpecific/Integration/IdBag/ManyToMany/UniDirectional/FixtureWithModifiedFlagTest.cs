using System;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;
using NHibernate.Cfg;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.IdBag.ManyToMany.UniDirectional
{
	public partial class FixtureWithModifiedFlagTest : TestBase
	{
		private Guid owningId;
		private Guid ownedId;

		public FixtureWithModifiedFlagTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true);
		}

		protected override void Initialize()
		{
			var owning = new UniOwning();
			var owned = new UniOwned { Number = 1 };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(owning);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				owning.Referencing.Add(owned);
				tx.Commit();
			}
			owningId = owning.Id;
			ownedId = owned.Id;
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			AuditReader().GetRevisions(typeof(UniOwning), owningId)
				.Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof(UniOwned), ownedId)
				.Should().Have.SameSequenceAs(2);
		}
	}
}
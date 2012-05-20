using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Strategy.Query
{
	[TestFixture]
	public class BasicQueriesTest : TestBase
	{
		private int id;

		public BasicQueriesTest(string strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.AuditStrategy, typeof(ValidityAuditStrategy));
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var obj = new StrTestEntity {Str = "test"};
				id = (int) Session.Save(obj);
				tx.Commit();
			}
		}

		[Test]
		public void HistoryQuery()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision<StrTestEntity>(1)
				.Results().First()
				.Should().Be.EqualTo(new StrTestEntity {Id = id, Str = "test"});
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"Entities.Mapping.hbm.xml"};
			}
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Query.Impl;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Strategy.Query
{
	[TestFixture]
	public class BasicQueries : TestBase
	{
		private int id;

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetProperty(ConfigurationKey.AuditStrategy, typeof (ValidityAuditStrategy).FullName);
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
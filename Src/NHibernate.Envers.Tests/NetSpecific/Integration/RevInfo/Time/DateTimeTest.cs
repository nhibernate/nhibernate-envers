using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.RevInfo.Time
{
	public partial class DateTimeTest : TestBase
	{
		public DateTimeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new StrTestEntity {Str = "data"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldUseUtcDateForRevisionDate()
		{
			var utcNow = DateTime.UtcNow;
			var res = AuditReader().CreateQuery().ForHistoryOf<StrTestEntity, DefaultRevisionEntity>().Results().First();
			res.RevisionEntity.RevisionDate.Should().Be.IncludedIn(utcNow.AddMinutes(-1), utcNow.AddMinutes(1));
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
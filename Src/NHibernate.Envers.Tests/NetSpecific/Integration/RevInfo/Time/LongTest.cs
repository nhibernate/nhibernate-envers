using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.RevEntity;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.RevInfo.Time
{
	public partial class LongTest : TestBase
	{
		public LongTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new StrTestEntity { Str = "data" };
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
			var res = AuditReader().CreateQuery().ForHistoryOf<StrTestEntity, CustomDataRevEntity>().Results().First();
			res.RevisionEntity.CustomTimestamp.Should().Be.IncludedIn(utcNow.AddMinutes(-1).Ticks, utcNow.AddMinutes(1).Ticks);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.CustomDataRevEntity.hbm.xml" };
			}
		} 
	}
}
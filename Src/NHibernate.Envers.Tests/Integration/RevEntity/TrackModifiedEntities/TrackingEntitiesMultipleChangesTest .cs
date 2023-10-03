using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	public partial class TrackingEntitiesMultipleChangesTest : TestBase
	{
		private int steId1;
		private int steId2;

		public TrackingEntitiesMultipleChangesTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ste1 = new StrTestEntity {Str = "x"};
			var ste2 = new StrTestEntity {Str = "y"};

			// Revision 1 - Adding two entities
			using (var tx = Session.BeginTransaction())
			{
				steId1 = (int) Session.Save(ste1);
				steId2 = (int) Session.Save(ste2);
				tx.Commit();
			}

			// Revision 2 - Changing first and removing second entity
			using (var tx = Session.BeginTransaction())
			{
				ste1.Str = "z";
				Session.Delete(ste2);
				tx.Commit();
			}

			// Revision 3 - Modifying and removing the same entity.
			using (var tx = Session.BeginTransaction())
			{
				ste1.Str = "a";
				Session.Delete(ste1);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldTrackAddedTwoEntities()
		{
			var ste1 = new StrTestEntity { Str = "x", Id = steId1 };
			var ste2 = new StrTestEntity { Str = "y", Id = steId2 };

			crossTypeRevisionChangesReader().FindEntities(1)
				.Should().Have.SameValuesAs(ste1, ste2);
		}

		[Test]
		public void ShouldTrackUpdateAndRemoveDifferentEntities()
		{
			var ste1 = new StrTestEntity { Str = "z", Id = steId1 };
			var ste2 = new StrTestEntity { Id = steId2 };

			crossTypeRevisionChangesReader().FindEntities(2)
				.Should().Have.SameValuesAs(ste1, ste2);
		}

		[Test]
		public void ShouldTrackUpdateAndRemoveTheSameEntity()
		{
			var ste1 = new StrTestEntity { Id = steId1 };

			crossTypeRevisionChangesReader().FindEntities(3)
				.Should().Have.SameValuesAs(ste1);
		}

		private ICrossTypeRevisionChangesReader crossTypeRevisionChangesReader()
		{
			return AuditReader().CrossTypeRevisionChangesReader();
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml" }; }
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.TrackEntitiesChangedInRevision, true);
		}
	}
}
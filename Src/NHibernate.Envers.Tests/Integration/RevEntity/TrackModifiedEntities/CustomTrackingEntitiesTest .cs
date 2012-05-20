using System.Collections.Generic;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	/// <summary>
	/// Tests proper behavior of entity listener that implements <see cref="IEntityTrackingRevisionListener"/>.
	/// interface. <see cref="CustomTrackingRevisionListener"/> shall be notified whenever an entity instance has been
	/// added, modified or removed, so that changed entity type can be persisted.
	/// </summary>
	public class CustomTrackingEntitiesTest : TestBase
	{
		public CustomTrackingEntitiesTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ste = new StrTestEntity{Str = "x"};
			var site = new StrIntTestEntity{Str = "y", Number = 1};

			// Revision 1 - Adding two entities
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ste);
				Session.Save(site);
				tx.Commit();
			}

			// Revision 2 - Modifying one entity
			using (var tx = Session.BeginTransaction())
			{
				site.Number = 2;
				tx.Commit();
			}

			// Revision 3 - Deleting both entities
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(ste);
				Session.Delete(site);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldTrackAddedEntities()
		{
			var steDescriptor = new ModifiedEntityTypeEntity {EntityName = typeof (StrTestEntity).FullName};
			var siteDescriptor = new ModifiedEntityTypeEntity {EntityName = typeof (StrIntTestEntity).FullName};

			var ctre = AuditReader().FindRevision<CustomTrackingRevisionEntity>(1);

			ctre.ModifiedEntityTypes
				.Should().Have.SameValuesAs(steDescriptor, siteDescriptor);
		}

		[Test]
		public void ShouldTrackModifiedEntities()
		{
			var siteDescriptor = new ModifiedEntityTypeEntity { EntityName = typeof(StrIntTestEntity).FullName };

			var ctre = AuditReader().FindRevision<CustomTrackingRevisionEntity>(2);

			ctre.ModifiedEntityTypes
				.Should().Have.SameValuesAs(siteDescriptor);
		}

		[Test]
		public void ShouldTrackDeletedEntities()
		{
			var steDescriptor = new ModifiedEntityTypeEntity { EntityName = typeof(StrTestEntity).FullName };
			var siteDescriptor = new ModifiedEntityTypeEntity { EntityName = typeof(StrIntTestEntity).FullName };

			var ctre = AuditReader().FindRevision<CustomTrackingRevisionEntity>(3);

			ctre.ModifiedEntityTypes
				.Should().Have.SameValuesAs(steDescriptor, siteDescriptor);
		}

		[Test]
		[ExpectedException(typeof(AuditException))]
		public void ShouldThrowFindEntitiesChangedInRevision()
		{
			AuditReader().CrossTypeRevisionChangesReader();
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.TrackModifiedEntities.CustomTrackingRevisionEntity.hbm.xml" }; }
		}
	}
}
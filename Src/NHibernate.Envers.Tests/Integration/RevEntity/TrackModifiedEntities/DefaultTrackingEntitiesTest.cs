using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	/// <summary>
	/// Tests proper behavior of tracking modified entity types when <code>nhibernate.envers.track_entities_changed_in_revision</code>
	/// parameter is set to <code>true</code>.
	/// </summary>
	[TestFixture]
	public class DefaultTrackingEntitiesTest : TestBase
	{
		private int steId;
		private int siteId;

		protected override void Initialize()
		{
			var ste = new StrTestEntity{Str = "x"};
			var site = new StrIntTestEntity {Str = "y", Number = 1};

			// Revision 1 - Adding two entities
			using (var tx = Session.BeginTransaction())
			{
				steId = (int) Session.Save(ste);
				siteId = (int) Session.Save(site);
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
		public void VerifyRevEntityTableCreation()
		{
			foreach (var classMapping in Cfg.CollectionMappings)
			{
				var table = classMapping.CollectionTable;
				if ("REVCHANGES".Equals(table.Name))
				{
					table.ColumnSpan.Should().Be.EqualTo(2);
					table.GetColumn(new Column("REV")).Should().Not.Be.Null();
					table.GetColumn(new Column("ENTITYTYPE")).Should().Not.Be.Null();
					return;
				}
			}
			Assert.Fail("Cannot find reventitiytable with correct columns!");
		}

		[Test]
		public void ShouldTrackAddedEntities()
		{
			var ste = new StrTestEntity{Str = "x", Id = steId};
			var site = new StrIntTestEntity {Str = "y", Number = 1, Id = siteId};

			AuditReader().FindEntitiesChangedInRevision(1)
				.Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public void ShouldTrackModifiedEntities()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			AuditReader().FindEntitiesChangedInRevision(2)
				.Should().Have.SameValuesAs(site);		
		}

		[Test]
		public void ShouldTrackDeletedEntities()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			AuditReader().FindEntitiesChangedInRevision(3)
				.Should().Have.SameValuesAs(site, ste);	
		}

		[Test]
		public void ShouldNotFindChangesInInvalidRevision()
		{
			AuditReader().FindEntitiesChangedInRevision(4)
				.Should().Be.Empty();
		}

		[Test]
		public void ShouldTrackAddedEntitiesGroupByRevisionType()
		{
			var ste = new StrTestEntity { Str = "x", Id = steId };
			var site = new StrIntTestEntity { Str = "y", Number = 1, Id = siteId };

			var result = AuditReader().FindEntitiesChangedInRevisionGroupByRevisionType(1);

			result[RevisionType.Added].Should().Have.SameValuesAs(site, ste);
			result[RevisionType.Modified].Should().Be.Empty();
			result[RevisionType.Deleted].Should().Be.Empty();
		}

		[Test]
		public void ShouldTrackModifiedEntitiesGroupByRevisionType()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			var result = AuditReader().FindEntitiesChangedInRevisionGroupByRevisionType(2);

			result[RevisionType.Added].Should().Be.Empty();
			result[RevisionType.Modified].Should().Have.SameValuesAs(site);
			result[RevisionType.Deleted].Should().Be.Empty();
		}

		[Test]
		public void ShouldTrackDeletedEntitiesGroupByRevisionType()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			var result = AuditReader().FindEntitiesChangedInRevisionGroupByRevisionType(3);

			result[RevisionType.Added].Should().Be.Empty();
			result[RevisionType.Modified].Should().Be.Empty();
			result[RevisionType.Deleted].Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public void ShouldFindChangedEntitiesByRevisionTypeAdd()
		{
			var ste = new StrTestEntity { Str = "x", Id = steId };
			var site = new StrIntTestEntity { Str = "y", Number = 1, Id = siteId };

			AuditReader().FindEntitiesChangedInRevision(1, RevisionType.Added)
				.Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public void ShouldFindChangedEntitiesByRevisionTypeModified()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			AuditReader().FindEntitiesChangedInRevision(2, RevisionType.Modified)
				.Should().Have.SameValuesAs(site);
		}

		[Test]
		public void ShouldFindChangedEntitiesByRevisionTypeDeleted()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			AuditReader().FindEntitiesChangedInRevision(3, RevisionType.Deleted)
				.Should().Have.SameValuesAs(site, ste);
		}

		[Test]
		public void ShouldFindEntityTypesChangedInRevision()
		{
			AuditReader().FindEntityTypesChangedInRevision(1)
				.Should().Have.SameValuesAs(typeof(StrTestEntity), typeof(StrIntTestEntity));

			AuditReader().FindEntityTypesChangedInRevision(2)
				.Should().Have.SameValuesAs(typeof(StrIntTestEntity));

			AuditReader().FindEntityTypesChangedInRevision(3)
				.Should().Have.SameValuesAs(typeof(StrTestEntity), typeof(StrIntTestEntity));
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"Entities.Mapping.hbm.xml"}; }
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetProperty(ConfigurationKey.TrackEntitiesChangedInRevision, "true");
		}
	}
}
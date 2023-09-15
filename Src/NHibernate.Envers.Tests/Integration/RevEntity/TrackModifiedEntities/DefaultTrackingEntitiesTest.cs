using System;
using System.Collections.Generic;
using NHibernate.Cfg;
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
	public partial class DefaultTrackingEntitiesTest : TestBase
	{
		private int steId;
		private int siteId;

		public DefaultTrackingEntitiesTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

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
					table.GetColumn(new Column("ENTITYNAME")).Should().Not.Be.Null();
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

			crossTypeRevisionChangesReader().FindEntities(1)
				.Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public void ShouldTrackModifiedEntities()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			crossTypeRevisionChangesReader().FindEntities(2)
				.Should().Have.SameValuesAs(site);		
		}

		[Test]
		public void ShouldTrackDeletedEntities()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			crossTypeRevisionChangesReader().FindEntities(3)
				.Should().Have.SameValuesAs(site, ste);	
		}

		[Test]
		public void ShouldNotFindChangesInInvalidRevision()
		{
			crossTypeRevisionChangesReader().FindEntities(4)
				.Should().Be.Empty();
		}

		[Test]
		public void ShouldTrackAddedEntitiesGroupByRevisionType()
		{
			var ste = new StrTestEntity { Str = "x", Id = steId };
			var site = new StrIntTestEntity { Str = "y", Number = 1, Id = siteId };

			var result = crossTypeRevisionChangesReader().FindEntitiesGroupByRevisionType(1);

			result[RevisionType.Added].Should().Have.SameValuesAs(site, ste);
			result[RevisionType.Modified].Should().Be.Empty();
			result[RevisionType.Deleted].Should().Be.Empty();
		}

		[Test]
		public void ShouldTrackModifiedEntitiesGroupByRevisionType()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			var result = crossTypeRevisionChangesReader().FindEntitiesGroupByRevisionType(2);

			result[RevisionType.Added].Should().Be.Empty();
			result[RevisionType.Modified].Should().Have.SameValuesAs(site);
			result[RevisionType.Deleted].Should().Be.Empty();
		}

		[Test]
		public void ShouldTrackDeletedEntitiesGroupByRevisionType()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			var result = crossTypeRevisionChangesReader().FindEntitiesGroupByRevisionType(3);

			result[RevisionType.Added].Should().Be.Empty();
			result[RevisionType.Modified].Should().Be.Empty();
			result[RevisionType.Deleted].Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public void ShouldFindChangedEntitiesByRevisionTypeAdd()
		{
			var ste = new StrTestEntity { Str = "x", Id = steId };
			var site = new StrIntTestEntity { Str = "y", Number = 1, Id = siteId };

			crossTypeRevisionChangesReader().FindEntities(1, RevisionType.Added)
				.Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public void ShouldFindChangedEntitiesByRevisionTypeModified()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			crossTypeRevisionChangesReader().FindEntities(2, RevisionType.Modified)
				.Should().Have.SameValuesAs(site);
		}

		[Test]
		public void ShouldFindChangedEntitiesByRevisionTypeDeleted()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			crossTypeRevisionChangesReader().FindEntities(3, RevisionType.Deleted)
				.Should().Have.SameValuesAs(site, ste);
		}

		[Test]
		public void ShouldFindEntityTypesChangedInRevision()
		{
			crossTypeRevisionChangesReader().FindEntityTypes(1)
				.Should().Have.SameValuesAs(createPair(typeof(StrTestEntity)), createPair(typeof(StrIntTestEntity)));

			crossTypeRevisionChangesReader().FindEntityTypes(2)
				.Should().Have.SameValuesAs(createPair(typeof(StrIntTestEntity)));

			crossTypeRevisionChangesReader().FindEntityTypes(3)
				.Should().Have.SameValuesAs(createPair(typeof(StrTestEntity)), createPair(typeof(StrIntTestEntity)));
		}

		private ICrossTypeRevisionChangesReader crossTypeRevisionChangesReader()
		{
			return AuditReader().CrossTypeRevisionChangesReader();
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"Entities.Mapping.hbm.xml"}; }
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.TrackEntitiesChangedInRevision, true);
		}

		private static Tuple<string, System.Type> createPair(System.Type type)
		{
			return new Tuple<string, System.Type>(type.FullName, type);
		}
	}
}
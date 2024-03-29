﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


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
	using System.Threading.Tasks;
	public partial class DefaultTrackingEntitiesTest : TestBase
	{

		[Test]
		public async Task ShouldTrackAddedEntitiesAsync()
		{
			var ste = new StrTestEntity{Str = "x", Id = steId};
			var site = new StrIntTestEntity {Str = "y", Number = 1, Id = siteId};

			(await (crossTypeRevisionChangesReader().FindEntitiesAsync(1)).ConfigureAwait(false))
				.Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public async Task ShouldTrackModifiedEntitiesAsync()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			(await (crossTypeRevisionChangesReader().FindEntitiesAsync(2)).ConfigureAwait(false))
				.Should().Have.SameValuesAs(site);		
		}

		[Test]
		public async Task ShouldTrackDeletedEntitiesAsync()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			(await (crossTypeRevisionChangesReader().FindEntitiesAsync(3)).ConfigureAwait(false))
				.Should().Have.SameValuesAs(site, ste);	
		}

		[Test]
		public async Task ShouldNotFindChangesInInvalidRevisionAsync()
		{
			(await (crossTypeRevisionChangesReader().FindEntitiesAsync(4)).ConfigureAwait(false))
				.Should().Be.Empty();
		}

		[Test]
		public async Task ShouldTrackAddedEntitiesGroupByRevisionTypeAsync()
		{
			var ste = new StrTestEntity { Str = "x", Id = steId };
			var site = new StrIntTestEntity { Str = "y", Number = 1, Id = siteId };

			var result = await (crossTypeRevisionChangesReader().FindEntitiesGroupByRevisionTypeAsync(1)).ConfigureAwait(false);

			result[RevisionType.Added].Should().Have.SameValuesAs(site, ste);
			result[RevisionType.Modified].Should().Be.Empty();
			result[RevisionType.Deleted].Should().Be.Empty();
		}

		[Test]
		public async Task ShouldTrackModifiedEntitiesGroupByRevisionTypeAsync()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			var result = await (crossTypeRevisionChangesReader().FindEntitiesGroupByRevisionTypeAsync(2)).ConfigureAwait(false);

			result[RevisionType.Added].Should().Be.Empty();
			result[RevisionType.Modified].Should().Have.SameValuesAs(site);
			result[RevisionType.Deleted].Should().Be.Empty();
		}

		[Test]
		public async Task ShouldTrackDeletedEntitiesGroupByRevisionTypeAsync()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			var result = await (crossTypeRevisionChangesReader().FindEntitiesGroupByRevisionTypeAsync(3)).ConfigureAwait(false);

			result[RevisionType.Added].Should().Be.Empty();
			result[RevisionType.Modified].Should().Be.Empty();
			result[RevisionType.Deleted].Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public async Task ShouldFindChangedEntitiesByRevisionTypeAddAsync()
		{
			var ste = new StrTestEntity { Str = "x", Id = steId };
			var site = new StrIntTestEntity { Str = "y", Number = 1, Id = siteId };

			(await (crossTypeRevisionChangesReader().FindEntitiesAsync(1, RevisionType.Added)).ConfigureAwait(false))
				.Should().Have.SameValuesAs(ste, site);
		}

		[Test]
		public async Task ShouldFindChangedEntitiesByRevisionTypeModifiedAsync()
		{
			var site = new StrIntTestEntity { Str = "y", Number = 2, Id = siteId };

			(await (crossTypeRevisionChangesReader().FindEntitiesAsync(2, RevisionType.Modified)).ConfigureAwait(false))
				.Should().Have.SameValuesAs(site);
		}

		[Test]
		public async Task ShouldFindChangedEntitiesByRevisionTypeDeletedAsync()
		{
			var ste = new StrTestEntity { Id = steId };
			var site = new StrIntTestEntity { Id = siteId };

			(await (crossTypeRevisionChangesReader().FindEntitiesAsync(3, RevisionType.Deleted)).ConfigureAwait(false))
				.Should().Have.SameValuesAs(site, ste);
		}
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	/// <summary>
	/// Tests proper behavior of revision entity that utilizes <see cref="ModifiedEntityNamesAttribute"/> annotation.
	/// </summary>
	[TestFixture]
	public class AnnotatedTrackingEntitiesTest : DefaultTrackingEntitiesTest
	{
		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetProperty(ConfigurationKey.TrackEntitiesChangedInRevision, "false");
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.TrackModifiedEntities.AnnotatedTrackingRevisionEntity.hbm.xml" }; }
		}
	}
}
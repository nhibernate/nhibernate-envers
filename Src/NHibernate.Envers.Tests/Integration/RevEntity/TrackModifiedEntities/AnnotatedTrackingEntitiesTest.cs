using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	/// <summary>
	/// Tests proper behavior of revision entity that utilizes <see cref="ModifiedEntityNamesAttribute"/> annotation.
	/// </summary>
	public class AnnotatedTrackingEntitiesTest : DefaultTrackingEntitiesTest
	{
		public AnnotatedTrackingEntitiesTest(string strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.TrackEntitiesChangedInRevision, false);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.TrackModifiedEntities.AnnotatedTrackingRevisionEntity.hbm.xml" }; }
		}
	}
}
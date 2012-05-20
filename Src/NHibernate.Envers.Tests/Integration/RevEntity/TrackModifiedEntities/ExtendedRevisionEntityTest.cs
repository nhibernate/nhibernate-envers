using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	/// <summary>
	/// Tests proper behavior of revision entity that extends <see cref="DefaultTrackingModifiedEntitiesRevisionEntity"/>.
	/// </summary>
	public class ExtendedRevisionEntityTest : DefaultTrackingEntitiesTest 
	{
		public ExtendedRevisionEntityTest(string strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.TrackModifiedEntities.ExtendedRevisionEntity.hbm.xml" }; }
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.TrackEntitiesChangedInRevision, false);
		}

		[Test]
		public void ShouldCommentPropertyValue()
		{
			var ere = AuditReader().FindRevision<ExtendedRevisionEntity>(1);
			ere.Comment
				.Should().Be.EqualTo(ExtendedRevisionListener.CommentValue);
		}
	}
}
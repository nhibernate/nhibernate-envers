﻿using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	[RevisionEntity(typeof(ExtendedRevisionListener))]
	public class ExtendedRevisionEntity : DefaultTrackingModifiedTypesRevisionEntity 
	{
		public virtual string Comment { get; set; }
	}
}
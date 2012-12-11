﻿using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.EntityInstantiation
{
	[AuditFactory(typeof(TestEntityFactory))]
	[Audited]
	public class FactoryCreatedTestEntity : TestEntityWithContext
	{
		public FactoryCreatedTestEntity()
		{
		}

		public FactoryCreatedTestEntity(bool createdByFactory)
			: base(createdByFactory)
		{
		}
	}
}

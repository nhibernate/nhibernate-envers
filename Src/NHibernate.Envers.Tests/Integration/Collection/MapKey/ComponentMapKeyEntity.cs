using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities.Components;

namespace NHibernate.Envers.Tests.Integration.Collection.MapKey
{
	public class ComponentMapKeyEntity
	{
		public ComponentMapKeyEntity()
		{
			IdMap = new Dictionary<Component1, ComponentTestEntity>();
		}

		public virtual int Id { get; set; }
		[Audited]
		public virtual IDictionary<Component1, ComponentTestEntity> IdMap { get; set; }
	}
}
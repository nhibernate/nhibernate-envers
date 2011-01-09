using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NHibernate.Envers.Compatibility.Attributes;

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
		[MapKey(Name = "Comp1")]
		public virtual IDictionary<Component1, ComponentTestEntity> IdMap { get; set; }
	}
}
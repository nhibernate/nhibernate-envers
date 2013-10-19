using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Components
{
	public class ComponentSetTestEntity
	{
		public ComponentSetTestEntity()
		{
			Comps = new HashSet<Component1>();
		}

		public virtual int Id { get; set; }

		[Audited]
		public virtual ISet<Component1> Comps { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as ComponentSetTestEntity;
			if (other == null)
				return false;
			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
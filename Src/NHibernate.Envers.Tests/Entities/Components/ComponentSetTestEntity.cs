using Iesi.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.Components
{
	public class ComponentSetTestEntity
	{
		public ComponentSetTestEntity()
		{
			Comps = new HashedSet<Component1>();
		}

		public virtual int Id { get; set; }

		//[Audited] todo: make test pass later
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
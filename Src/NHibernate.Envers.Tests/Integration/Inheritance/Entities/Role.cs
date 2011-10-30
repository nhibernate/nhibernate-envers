using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class Role : RightsSubject
	{
		public Role()
		{
			Name = string.Empty;
			Members = new HashedSet<RightsSubject>();
		}

		public virtual string Name { get; set; }
		public virtual ISet<RightsSubject> Members { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as Role;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return Name.Equals(casted.Name);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ Name.GetHashCode();
		}
	}
}
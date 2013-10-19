using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class Role : RightsSubject
	{
		public Role()
		{
			Name = string.Empty;
			Members = new HashSet<RightsSubject>();
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

		public override string ToString()
		{
			return string.Format("Id: {0}, Name: {1}, Group: {2}", Id, Name, Group);
		}
	}
}
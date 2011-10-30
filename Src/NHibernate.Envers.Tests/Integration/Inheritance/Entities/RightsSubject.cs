using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class RightsSubject
	{
		public RightsSubject()
		{
			Group = string.Empty;
			Roles = new HashedSet<Role>();
		}

		public virtual long Id { get; set; }
		public virtual long Version { get; set; }
		public virtual string Group { get; set; }
		public virtual ISet<Role> Roles { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as RightsSubject;
			if (casted == null)
				return false;
			return Id == casted.Id && Group.Equals(casted.Group);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^Group.GetHashCode();
		}
	}
}
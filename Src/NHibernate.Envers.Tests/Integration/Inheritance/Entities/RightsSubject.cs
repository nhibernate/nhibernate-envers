using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class RightsSubject
	{
		public RightsSubject()
		{
			Group = string.Empty;
			Roles = new HashSet<Role>();
		}

		public virtual long Id { get; set; }
		public virtual long Version { get; set; }
		public virtual string Group { get; set; }
		public virtual ISet<Role> Roles { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as RightsSubject;
			if (that == null)
				return false;
			if (string.IsNullOrEmpty(Group) ? !string.IsNullOrEmpty(that.Group) : !Group.Equals(that.Group)) return false;
			return Id.Equals(that.Id);
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31 * result + (string.IsNullOrEmpty(Group) ? 0 : Group.GetHashCode());
			return result;
		}
	}
}
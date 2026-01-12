using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Tree
{
	[Audited]
	public class TreeEntity
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual TreeEntity Parent { get; set; }
		public virtual IList<TreeEntity> Children { get; protected set; } = new List<TreeEntity>();
		
		public override bool Equals(object obj)
		{
			if (!(obj is TreeEntity other))
				return false;
			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
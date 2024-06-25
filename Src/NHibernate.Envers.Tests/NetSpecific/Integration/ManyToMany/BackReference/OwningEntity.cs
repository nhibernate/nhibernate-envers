using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ManyToMany.BackReference
{
	[Audited]
	public class OwningEntity
	{
		public virtual long Id { get; set; }

		public virtual IList<BackReferenceEntity> Items { get; protected set; } = new List<BackReferenceEntity>();
		
		public override bool Equals(object obj)
		{
			if (!(obj is OwningEntity other))
				return false;
			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
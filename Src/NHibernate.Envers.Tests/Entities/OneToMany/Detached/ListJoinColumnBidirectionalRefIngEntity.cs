using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	[Audited]
	public class ListJoinColumnBidirectionalRefIngEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		public virtual IList<ListJoinColumnBidirectionalRefEdEntity> References { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ListJoinColumnBidirectionalRefIngEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}
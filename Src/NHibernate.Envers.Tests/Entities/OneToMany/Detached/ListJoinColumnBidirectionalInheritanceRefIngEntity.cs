using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	[Audited]
	public class ListJoinColumnBidirectionalInheritanceRefIngEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		[AuditMappedBy(MappedBy = "Owner")]
		public virtual IList<ListJoinColumnBidirectionalInheritanceRefEdParentEntity> References { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ListJoinColumnBidirectionalInheritanceRefIngEntity;
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
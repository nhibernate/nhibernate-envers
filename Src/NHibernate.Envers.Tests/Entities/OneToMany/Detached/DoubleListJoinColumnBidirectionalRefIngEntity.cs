using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	[Audited]
	public class DoubleListJoinColumnBidirectionalRefIngEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		[AuditMappedBy(MappedBy = "Owner")]
		public virtual IList<DoubleListJoinColumnBidirectionalRefEdEntity1> References1 { get; set; }
		[AuditMappedBy(MappedBy = "Owner")]
		public virtual IList<DoubleListJoinColumnBidirectionalRefEdEntity2> References2 { get; set; }

		public DoubleListJoinColumnBidirectionalRefIngEntity()
		{
			References1 = new List<DoubleListJoinColumnBidirectionalRefEdEntity1>();
			References2 = new List<DoubleListJoinColumnBidirectionalRefEdEntity2>();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as DoubleListJoinColumnBidirectionalRefIngEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data == casted.Data);
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}
namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	[Audited]
	public class ListJoinColumnBidirectionalInheritanceRefEdParentEntity
	{
		public virtual int Id { get; set; }
		public virtual string ParentData { get; set; }
		public virtual ListJoinColumnBidirectionalInheritanceRefIngEntity Owner { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ListJoinColumnBidirectionalInheritanceRefEdParentEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && ParentData.Equals(casted.ParentData));
		}

		public override int GetHashCode()
		{
			return Id ^ ParentData.GetHashCode();
		}
	}
}
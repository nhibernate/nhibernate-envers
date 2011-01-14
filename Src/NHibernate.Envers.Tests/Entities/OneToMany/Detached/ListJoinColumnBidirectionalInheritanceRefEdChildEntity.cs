namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	[Audited]
	public class ListJoinColumnBidirectionalInheritanceRefEdChildEntity : ListJoinColumnBidirectionalInheritanceRefEdParentEntity
	{
		public virtual string ChildData { get; set; }

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
				return false;
			var casted = obj as ListJoinColumnBidirectionalInheritanceRefEdChildEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && ChildData.Equals(casted.ChildData));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ParentData.GetHashCode();
		}
	}
}
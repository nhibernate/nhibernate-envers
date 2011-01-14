namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	[Audited]
	public class ListJoinColumnBidirectionalRefEdEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		public virtual ListJoinColumnBidirectionalRefIngEntity Owner { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ListJoinColumnBidirectionalRefEdEntity;
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
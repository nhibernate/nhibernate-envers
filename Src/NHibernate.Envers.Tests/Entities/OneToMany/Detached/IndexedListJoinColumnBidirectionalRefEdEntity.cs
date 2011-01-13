namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	[Audited]
	public class IndexedListJoinColumnBidirectionalRefEdEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		public virtual IndexedListJoinColumnBidirectionalRefIngEntity Owner { get; set; }
		public virtual int Position { get; private set; }

		public override bool Equals(object obj)
		{
			var casted = obj as IndexedListJoinColumnBidirectionalRefEdEntity;
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
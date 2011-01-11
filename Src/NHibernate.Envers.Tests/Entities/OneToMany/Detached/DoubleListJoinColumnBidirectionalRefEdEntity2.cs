namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	public class DoubleListJoinColumnBidirectionalRefEdEntity2
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual DoubleListJoinColumnBidirectionalRefIngEntity Owner { get; set; }


		public override bool Equals(object obj)
		{
			var casted = obj as DoubleListJoinColumnBidirectionalRefEdEntity2;
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
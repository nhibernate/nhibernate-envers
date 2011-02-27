using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	public class DoubleListJoinColumnBidirectionalRefEdEntity1
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual DoubleListJoinColumnBidirectionalRefIngEntity Owner { get; set; }


		public override bool Equals(object obj)
		{
			var casted = obj as DoubleListJoinColumnBidirectionalRefEdEntity1;
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
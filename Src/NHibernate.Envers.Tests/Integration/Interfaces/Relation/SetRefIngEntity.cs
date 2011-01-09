namespace NHibernate.Envers.Tests.Integration.Interfaces.Relation
{
	public class SetRefIngEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }
		[Audited]
		public virtual ISetRefEdEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as SetRefIngEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Data.GetHashCode();
		}
	}
}
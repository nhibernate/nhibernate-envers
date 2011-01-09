namespace NHibernate.Envers.Tests.Integration.Interfaces.Relation
{
	[Audited]
	public class SetRefEdEntity : ISetRefEdEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as SetRefEdEntity;
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
namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.Relation.Unidirectional
{
	[Audited]
	public abstract class AbstractContainedEntity
	{
		public virtual long Id { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as AbstractContainedEntity;
			if (casted == null)
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class ChildPrimaryKeyJoinEntity : ParentEntity
	{
		public virtual long Number { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ChildPrimaryKeyJoinEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj)) return false;
			return Number == casted.Number;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ Number.GetHashCode();
		}
	}
}
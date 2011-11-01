namespace NHibernate.Envers.Tests.Integration.Inheritance.Single.DiscriminatorFormula
{
	public class ClassTypeEntity
	{
		public const string ParentType = "parent";
		public const string ChildType = "child";
		public virtual long Id { get; set; }
		public virtual string Type { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ClassTypeEntity;
			if(casted==null)
				return false;
			return Id == casted.Id && Type == casted.Type;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Type.GetHashCode();
		}
	}
}
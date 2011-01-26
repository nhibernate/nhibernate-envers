namespace NHibernate.Envers.Tests.Integration.SuperClass
{
	public class SubclassEntity : SuperclassOfEntity
	{
		public virtual int Id { get; set;}

		public override bool Equals(object obj)
		{
			var casted = obj as SubclassEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id ^ base.GetHashCode();
		}
	}
}
namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class ChildNotIngEntity : ParentIngEntity
	{
		public virtual long Number { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ChildNotIngEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return (Number == casted.Number);
		}

		public override int GetHashCode()
		{
			var res = base.GetHashCode();
			res = res ^ Number.GetHashCode();
			return res;
		}
	}
}
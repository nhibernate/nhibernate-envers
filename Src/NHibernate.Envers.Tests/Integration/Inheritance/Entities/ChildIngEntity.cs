namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class ChildIngEntity : ParentNotIngEntity
	{
		public virtual int Number { get; set; }
		public virtual ReferencedToChildEntity Referenced { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ChildIngEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return (Id == casted.Id && Number == casted.Number);
		}

		public override int GetHashCode()
		{
			var res = base.GetHashCode();
			res = res ^ Number.GetHashCode() ^ Referenced.GetHashCode();
			return res;
		}
	}
}
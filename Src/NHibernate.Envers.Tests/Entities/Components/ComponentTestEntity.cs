namespace NHibernate.Envers.Tests.Entities.Components
{
	public class ComponentTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual Component1 Comp1 { get; set; }
		public virtual Component2 Comp2 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as ComponentTestEntity;
			if (other == null)
				return false;
			if (Comp1 != null ? !Comp1.Equals(other.Comp1) : other.Comp1 != null) return false;
			if (Comp2 != null ? !Comp2.Equals(other.Comp2) : other.Comp2 != null) return false;
			return Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31 * result + (Comp1 != null ? Comp1.GetHashCode() : 0);
			result = 31 * result + (Comp2 != null ? Comp2.GetHashCode() : 0);
			return result;
		}
	}
}
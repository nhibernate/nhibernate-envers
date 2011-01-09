namespace NHibernate.Envers.Tests.Integration.Interfaces.Components
{
	[Audited]
	public class ComponentTestEntity
	{
		public virtual int Id { get; set; }
		public virtual IComponent Comp1 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ComponentTestEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Comp1.Equals(casted.Comp1));
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Comp1.GetHashCode();
		}
	}
}
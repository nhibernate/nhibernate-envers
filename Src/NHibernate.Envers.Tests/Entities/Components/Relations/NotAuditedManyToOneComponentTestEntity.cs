namespace NHibernate.Envers.Tests.Entities.Components.Relations
{
	public class NotAuditedManyToOneComponentTestEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual NotAuditedManyToOneComponent Comp1 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as NotAuditedManyToOneComponentTestEntity;
			if (other == null)
				return false;

			if (Comp1 != null ? !Comp1.Equals(other.Comp1) : other.Comp1 != null) return false;
			if (!Id.Equals(other.Id)) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Comp1.GetHashCode();
		}
	}
}
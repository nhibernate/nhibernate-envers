namespace NHibernate.Envers.Tests.Entities.Components.Relations
{
	public class OneToManyComponentTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual OneToManyComponent Comp1 { get; set; }

		public override bool Equals(object o)
		{
			var other = o as OneToManyComponentTestEntity;
			if(other==null)
				return false;

			if (Comp1 != null ? !Comp1.Equals(other.Comp1) : other.Comp1 != null) return false;
			if (!Id.Equals(other.Id)) return false;

			return true;
		}

		public override int GetHashCode() 
		{
			var result = Id.GetHashCode();
			result = 31 * result + (Comp1 != null ? Comp1.GetHashCode() : 0);
			return result;
		}
	}
}
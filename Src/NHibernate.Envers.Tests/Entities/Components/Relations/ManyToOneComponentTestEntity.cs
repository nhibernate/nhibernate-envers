namespace NHibernate.Envers.Tests.Entities.Components.Relations
{
	public class ManyToOneComponentTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual ManyToOneComponent Comp1 { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as ManyToOneComponentTestEntity;
			if(that ==null)
			{
				return false;
			}

			return Id == that.Id && Comp1.Equals(that.Comp1);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Comp1.GetHashCode();
		}
	}
}
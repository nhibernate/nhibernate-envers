using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Components
{
	[Audited]
	public class DefaultValueComponentTestEntity
	{
		public virtual int Id { get; set; }
		public virtual DefaultValueComponent1 Comp1 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as DefaultValueComponentTestEntity;
			if (casted==null)
			{
				return false;
			}
			if (Id != casted.Id)
				return false;
			if (Comp1 != null ? !Comp1.Equals(casted.Comp1) : casted.Comp1 != null)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var compHash = Comp1 != null ? Comp1.GetHashCode() : 0;
			return Id ^ compHash;
		}
	}
}
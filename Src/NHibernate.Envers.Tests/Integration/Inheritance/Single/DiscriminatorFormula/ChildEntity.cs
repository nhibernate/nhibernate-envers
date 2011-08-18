using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Single.DiscriminatorFormula
{
	[Audited]
	public class ChildEntity : ParentEntity
	{
		public virtual string SpecificData { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ChildEntity;
			if (casted == null || !base.Equals(casted))
				return false;
			return SpecificData.Equals(casted.SpecificData);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ SpecificData.GetHashCode();
		} 
	}
}
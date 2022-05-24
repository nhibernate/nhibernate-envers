using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Inheritance.Discriminator.Discriminate
{
	[Audited]
	public class SubtypeEntity : BaseEntity
	{
		public virtual string SubtypeData { get; set; }

		public override bool Equals(object obj)
		{
	 		var casted = obj as SubtypeEntity;
			if (casted == null)
				return false;

			return base.Equals(casted)
				&& string.Equals(SubtypeData, casted.SubtypeData);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		} 
	}
}
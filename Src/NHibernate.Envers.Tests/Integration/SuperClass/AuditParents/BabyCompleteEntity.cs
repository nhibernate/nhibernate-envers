using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditParents
{
	[Audited(AuditParents = new[] { typeof(MappedParentEntity) })]
	public class BabyCompleteEntity : ChildCompleteEntity
	{
		public virtual string Baby { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as BabyCompleteEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return Baby != null ? Baby.Equals(casted.Baby) : casted.Baby == null;
		}

		public override int GetHashCode()
		{
			var result = base.GetHashCode();
			result = 31 * result + (Baby != null ? Baby.GetHashCode() : 0);
			return result;
		}
	}
}
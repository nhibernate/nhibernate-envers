using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditParents
{
	[Audited(AuditParents = new[] { typeof(MappedGrandParentEntity) })]
	public class TransitiveParentEntity : MappedGrandParentEntity
	{
		public virtual string Parent { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as TransitiveParentEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return Parent != null ? Parent.Equals(casted.Parent) : casted.Parent == null;
		}

		public override int GetHashCode()
		{
			var result = base.GetHashCode();
			result = 31 * result + (Parent != null ? Parent.GetHashCode() : 0);
			return result;
		}
	}
}
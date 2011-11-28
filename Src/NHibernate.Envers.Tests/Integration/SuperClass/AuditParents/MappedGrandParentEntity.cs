using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditParents
{
	public class MappedGrandParentEntity
	{
		public virtual long Id { get; set; }
		public virtual string GrandParent { get; set; }
		[NotAudited]
		public virtual string NotAudited { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as MappedGrandParentEntity;
			if (casted == null)
				return false;
			if (GrandParent != null ? !GrandParent.Equals(casted.GrandParent) : casted.GrandParent != null) return false;
			if (NotAudited != null ? !NotAudited.Equals(casted.NotAudited) : casted.NotAudited != null) return false;

			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31 * result + (GrandParent != null ? GrandParent.GetHashCode() : 0);
			result = 31 * result + (NotAudited != null ? NotAudited.GetHashCode() : 0);
			return result; 
		}
	}
}
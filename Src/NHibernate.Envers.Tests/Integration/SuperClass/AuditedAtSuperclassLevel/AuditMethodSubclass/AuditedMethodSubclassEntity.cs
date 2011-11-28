using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditedAtSuperclassLevel.AuditMethodSubclass
{
	public class AuditedMethodSubclassEntity : AuditedAllMappedSuperclass
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string SubAuditedStr { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as AuditedMethodSubclassEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id ^ base.GetHashCode();
		}
	}
}
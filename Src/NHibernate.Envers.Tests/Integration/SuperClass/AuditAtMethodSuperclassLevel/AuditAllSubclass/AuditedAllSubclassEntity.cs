using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditAtMethodSuperclassLevel.AuditAllSubclass
{
	[Audited]
	public class AuditedAllSubclassEntity : AuditedMethodMappedSuperclass
	{
		public virtual int Id { get; set; }
		public virtual string SubAuditedStr { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as AuditedAllSubclassEntity;
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
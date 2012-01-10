using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride
{
	[Audited]
	[AuditOverride(PropertyName = "Str1", IsAudited = true)]
	[AuditOverride(PropertyName = "Number2", IsAudited = true)]
	public class TransitiveOverrideTestEntity : ExtendedBaseEntity
	{
		public virtual string Str3 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as TransitiveOverrideTestEntity;
			if (other == null)
				return false;
			if (!base.Equals(obj))
				return false;

			return Str3 == null ? other.Str3 == null : Str3.Equals(other.Str3);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (Str3 == null ? 0 : Str3.GetHashCode());
		}
	}
}
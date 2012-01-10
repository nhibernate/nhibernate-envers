using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride
{
	[AuditOverride(PropertyName = "Str1", IsAudited = false)]
	[AuditOverride(PropertyName = "Number1", IsAudited = true)]
	public class ExtendedBaseEntity : BaseEntity
	{
		[Audited]
		public virtual string Str2 { get; set; }

		[NotAudited]
		public virtual int Number2 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as ExtendedBaseEntity;
			if (other == null)
				return false;
			if (!base.Equals(obj))
				return false;

			if (Number2 != other.Number2)
				return false;
			return Str2 == null ? other.Str2 == null : Str2.Equals(other.Str2);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (Str2 == null ? 0 : Str2.GetHashCode()) ^Number2;
		}
	}
}
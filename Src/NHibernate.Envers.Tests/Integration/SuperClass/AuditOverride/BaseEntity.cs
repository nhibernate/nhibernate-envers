using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride
{
	public class BaseEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Str1 { get; set; }

		[NotAudited]
		public virtual int Number1 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as BaseEntity;
			if (other == null)
				return false;
			if (Id != other.Id)
				return false;
			if (Number1 != other.Number1)
				return false;
			return Str1 == null ? other.Str1 == null : Str1.Equals(other.Str1);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Number1 ^ (Str1 == null ? 0 : Str1.GetHashCode());
		}
	}
}
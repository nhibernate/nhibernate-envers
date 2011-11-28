using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditedAtSuperclassLevel
{
	[Audited]
	public class AuditedAllMappedSuperclass
	{
		public virtual string Str { get; set; }
		public virtual string OtherStr { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as AuditedAllMappedSuperclass;
			if (casted == null)
				return false;

			if (Str != null ? !Str.Equals(casted.Str) : casted.Str != null)
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			return (Str != null ? Str.GetHashCode() : 0);
		}
	}
}
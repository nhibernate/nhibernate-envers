using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Components
{
	[Audited]
	public class Component3
	{
		public virtual string Str1 { get; set; }
		public Component4 AuditedComponent { get; set; }
		[NotAudited]
		public Component4 NonAuditedComponent { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as Component3;
			if (other == null)
				return false;
			if (AuditedComponent != null ? !AuditedComponent.Equals(other.AuditedComponent) : other.AuditedComponent != null) return false;
			if (Str1 != null ? !Str1.Equals(other.Str1) : other.Str1 != null) return false;
			return true;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			var result = 1;
			result = prime * result + ((AuditedComponent == null) ? 0 : AuditedComponent.GetHashCode());
			result = prime * result + ((Str1 == null) ? 0 : Str1.GetHashCode());
			return result;
		}
	}
}
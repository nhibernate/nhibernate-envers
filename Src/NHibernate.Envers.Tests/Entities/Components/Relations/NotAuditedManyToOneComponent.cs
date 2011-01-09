using System;

namespace NHibernate.Envers.Tests.Entities.Components.Relations
{
	public class NotAuditedManyToOneComponent
	{
		public string Data { get; set; }
		[NotAudited]
		public UnversionedStrTestEntity Entity { get; set; }

		// override object.Equals
		public override bool Equals(object obj)
		{
			var other = obj as NotAuditedManyToOneComponent;
			if (other == null)
				return false;

			if (Entity != null ? !Entity.Equals(other.Entity) : other.Entity != null) return false;
			if (!Data.Equals(other.Data)) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return Data.GetHashCode() ^ Entity.GetHashCode();
		}
	}
}
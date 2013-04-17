using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional
{
	public class M2MIndexedListTargetNotAuditedEntity
	{
		public M2MIndexedListTargetNotAuditedEntity()
		{
			References = new List<UnversionedStrTestEntity>();
		}

		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual IList<UnversionedStrTestEntity> References { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as M2MIndexedListTargetNotAuditedEntity;
			if (that == null)
				return false;
			return Data != null ? Data.Equals(that.Data) : that.Data == null;
		}

		public override int GetHashCode()
		{
			return Data != null ? Data.GetHashCode() : 0;
		}
	}
}
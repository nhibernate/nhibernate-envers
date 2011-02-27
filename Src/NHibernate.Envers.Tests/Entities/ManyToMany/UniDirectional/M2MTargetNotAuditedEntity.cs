using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional
{
	public class M2MTargetNotAuditedEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual IList<UnversionedStrTestEntity> References { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ListOwnedEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data == casted.Data);
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}
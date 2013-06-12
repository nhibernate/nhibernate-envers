using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional
{
	[Audited]
	public class ManyToManyNotAuditedNullEntity
	{
		protected ManyToManyNotAuditedNullEntity()
		{
		}

		public ManyToManyNotAuditedNullEntity(int id, string data)
		{
			References = new List<UnversionedStrTestEntity>();
			Id = id;
			Data = data;
		}
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual IList<UnversionedStrTestEntity> References { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as ManyToManyNotAuditedNullEntity;
			if (that == null)
				return false;
			if (Data != null ? !Data.Equals(that.Data) : that.Data != null) 
				return false;
			return Id == that.Id;
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			return 31*result + (Data != null ? Data.GetHashCode() : 0);
		}
	}
}
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToOne.UniDirectional
{
	[Audited]
	public class ManyToOneNotAuditedNullEntity
	{
		protected ManyToOneNotAuditedNullEntity(){}

		public ManyToOneNotAuditedNullEntity(int id, string data, UnversionedStrTestEntity reference)
		{
			Reference = reference;
			Id = id;
			Data = data;
		}
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual UnversionedStrTestEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as ManyToOneNotAuditedNullEntity;
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
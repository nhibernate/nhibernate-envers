namespace NHibernate.Envers.Tests.Entities.ManyToOne.UniDirectional
{
	public class TargetNotAuditedEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited(TargetAuditMode = RelationTargetAuditMode.NOT_AUDITED)]
		public virtual UnversionedStrTestEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as TargetNotAuditedEntity;
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
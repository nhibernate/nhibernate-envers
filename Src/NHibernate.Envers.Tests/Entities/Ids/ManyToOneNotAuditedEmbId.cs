namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class ManyToOneNotAuditedEmbId
	{
		public virtual UnversionedStrTestEntity Id { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ManyToOneNotAuditedEmbId;
			if (casted == null)
				return false;
			if (Id != null ? !Id.Equals(casted.Id) : casted.Id != null) 
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			return Id != null ? Id.GetHashCode() : 0;
		}
	}
}
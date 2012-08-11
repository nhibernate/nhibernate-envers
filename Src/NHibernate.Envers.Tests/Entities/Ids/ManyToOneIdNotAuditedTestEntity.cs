namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class ManyToOneIdNotAuditedTestEntity
	{
		public virtual ManyToOneNotAuditedEmbId Id { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as ManyToOneIdNotAuditedTestEntity;
			if (that == null)
				return false;
			if (Data != null ? !Data.Equals(that.Data) : that.Data != null) 
				return false;
			if (Id != null ? !Id.Equals(that.Id) : that.Id != null) 
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			var result = Id != null ? Id.GetHashCode() : 0;
			result = 31 * result + (Data != null ? Data.GetHashCode() : 0);
			return result;
		}
	}
}
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags.Entities
{
	[Audited(WithModifiedFlag = true)]
	public class WithModifiedFlagReferencingEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		public virtual PartialModifiedFlagsEntity Reference { get; set; }
		public virtual PartialModifiedFlagsEntity SecondReference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as WithModifiedFlagReferencingEntity;
			if (casted == null)
				return false;
			if (Data != null ? !Data.Equals(casted.Data) : casted.Data != null)
				return false;
			if (Id != casted.Id)
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			var result = Id;
			result = 31 * result + (Data != null ? Data.GetHashCode() : 0);
			return result;
		}
	}
}
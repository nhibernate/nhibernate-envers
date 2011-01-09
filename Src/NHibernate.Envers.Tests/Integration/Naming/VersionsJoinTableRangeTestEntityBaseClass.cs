namespace NHibernate.Envers.Tests.Integration.Naming
{
	[Audited]
	public abstract class VersionsJoinTableRangeTestEntityBaseClass
	{
		public virtual int Id { get; set; }
		public virtual string GenericValue { get; set; }

		public override bool Equals(object obj)
		{
			if (GetType() != obj.GetType())
				return false;
			var casted = (VersionsJoinTableRangeTestEntityBaseClass)obj;
			return (Id == casted.Id && GenericValue.Equals(casted.GenericValue));
		}

		public override int GetHashCode()
		{
			return Id ^ GenericValue.GetHashCode();
		}
	}
}
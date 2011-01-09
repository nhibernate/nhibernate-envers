namespace NHibernate.Envers.Tests.Integration.Naming
{
	[Audited]
	public class VersionsJoinTableRangeTestEntity : VersionsJoinTableRangeTestEntityBaseClass
	{
		public virtual string Value { get; set; }

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
				return false;
			var casted = (VersionsJoinTableRangeTestEntity)obj;
			return Value.Equals(casted.Value);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ Value.GetHashCode();
		}
	}
}
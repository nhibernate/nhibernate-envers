namespace NHibernate.Envers.Tests.Integration.Naming
{
	[Audited]
	public class VersionsJoinTableRangeTestAlternateEntity : VersionsJoinTableRangeTestEntityBaseClass
	{
		public virtual string AlternativeValue { get; set; }

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
				return false;
			var casted = (VersionsJoinTableRangeTestAlternateEntity)obj;
			return AlternativeValue.Equals(casted.AlternativeValue);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ AlternativeValue.GetHashCode();
		}
	}
}
using NHibernate.Envers.Tests.Entities.Components;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public class VersionsJoinTableRangeComponentTestEntity
	{
		public VersionsJoinTableRangeComponentTestEntity()
		{
			Component1 = new VersionsJoinTableRangeComponent<VersionsJoinTableRangeTestEntity>();
			Component2 = new VersionsJoinTableRangeComponent<VersionsJoinTableRangeTestAlternateEntity>();
			Component3 = new Component1();
		}

		public virtual int Id { get; set; }
		[Audited]
		public virtual VersionsJoinTableRangeComponent<VersionsJoinTableRangeTestEntity> Component1 { get; set; }
		[Audited]
		public virtual VersionsJoinTableRangeComponent<VersionsJoinTableRangeTestAlternateEntity> Component2 { get; set; }
		[Audited]
		[AuditOverride(Name="Str2", IsAudited = false)]
		public virtual Component1 Component3 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as VersionsJoinTableRangeComponentTestEntity;
			if (casted == null)
				return false;
			return Id == casted.Id && 
					Component1.Equals(casted.Component1) &&
					Component2.Equals(casted.Component2) &&
					Component3.Equals(casted.Component3);
		}

		public override int GetHashCode()
		{
			return Id ^ Component1.GetHashCode() ^ Component2.GetHashCode() ^ Component3.GetHashCode();
		}
	}
}
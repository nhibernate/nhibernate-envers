using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities.Components;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public class VersionsJoinTableRangeComponentTestEntity
	{
		public VersionsJoinTableRangeComponentTestEntity()
		{
			Component1 = new VersionsJoinTableRangeComponent<VersionsJoinTableRangeTestEntity>();
			Component2 = new VersionsJoinTableRangeComponent<VersionsJoinTableRangeTestAlternateEntity>();
		}

		public virtual int Id { get; set; }

		[Audited]
		[AuditOverride(PropertyName = "Range", TableName = "JOIN_TABLE_COMPONENT_1_AUD", InverseJoinColumns = new[] { "VJTRTE_ID" })]
		public virtual VersionsJoinTableRangeComponent<VersionsJoinTableRangeTestEntity> Component1 { get; set; }

		[Audited]
		[AuditOverride(PropertyName = "Range", TableName = "JOIN_TABLE_COMPONENT_2_AUD", InverseJoinColumns = new[] { "VJTRTAE_ID" })]
		public virtual VersionsJoinTableRangeComponent<VersionsJoinTableRangeTestAlternateEntity> Component2 { get; set; }

		[Audited]
		[AuditOverride(PropertyName = "Str2", IsAudited = false)]
		public virtual Component1 Component3 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as VersionsJoinTableRangeComponentTestEntity;
			if (casted == null)
				return false;
			if (Id != casted.Id)
				return false;
			if (Component1 == null)
			{
				if (casted.Component1 != null)
					return false;
			}
			else if (!Component1.Equals(casted.Component1))
			{
				return false;
			}
			if (Component2 == null)
			{
				if (casted.Component2 != null)
					return false;
			}
			else if (!Component2.Equals(casted.Component1))
			{
				return false;
			}
			if (Component3 == null)
			{
				if (casted.Component3 != null)
					return false;
			}
			else if (!Component3.Equals(casted.Component1))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			var h1 = Component1 == null ? 0 : Component1.GetHashCode();
			var h2 = Component2 == null ? 0 : Component2.GetHashCode();
			var h3 = Component3 == null ? 0 : Component3.GetHashCode();
			return Id ^ h1 ^ h2 ^ h3;
		}
	}
}
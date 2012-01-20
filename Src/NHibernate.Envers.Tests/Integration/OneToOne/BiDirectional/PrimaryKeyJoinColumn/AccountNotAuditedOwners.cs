using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.PrimaryKeyJoinColumn
{
	[Audited]
	public class AccountNotAuditedOwners
	{
		public virtual long AccountId { get; set; }
		public virtual string Type { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual NotAuditedNoProxyPerson Owner { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual NotAuditedProxyPerson CoOwner { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as AccountNotAuditedOwners;
			if (casted == null)
				return false;
			if (AccountId != casted.AccountId)
				return false;
			if (Type != null ? !Type.Equals(casted.Type) : casted.Type != null)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var typeResult = Type != null ? Type.GetHashCode() : 0;
			return (int)AccountId ^ typeResult;
		}
	}
}
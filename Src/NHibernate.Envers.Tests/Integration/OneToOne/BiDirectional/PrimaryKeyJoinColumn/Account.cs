using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.PrimaryKeyJoinColumn
{
	[Audited]
	public class Account
	{
		public virtual long AccountId { get; set; }
		public virtual string Type { get; set; }
		public virtual Person Owner { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as Account;
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
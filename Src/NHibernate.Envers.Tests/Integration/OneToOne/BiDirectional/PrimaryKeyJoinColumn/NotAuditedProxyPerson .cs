namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.PrimaryKeyJoinColumn
{
	public class NotAuditedProxyPerson 
	{
		public virtual long PersonId { get; set; }
		public virtual string Name { get; set; }
		public virtual AccountNotAuditedOwners Account { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as NotAuditedProxyPerson;
			if (casted == null)
				return false;
			if (PersonId != casted.PersonId)
				return false;
			if (Name != null ? !Name.Equals(casted.Name) : casted.Name != null)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var typeResult = Name != null ? Name.GetHashCode() : 0;
			return (int)PersonId ^ typeResult;
		}
	}
}
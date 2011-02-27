using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToOne
{
	[Audited]    
	public class OneToOneOwningEntity
	{
		public virtual int Id { get; set; }
		public virtual OneToOneOwnedEntity Owned { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as OneToOneOwningEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id);
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}
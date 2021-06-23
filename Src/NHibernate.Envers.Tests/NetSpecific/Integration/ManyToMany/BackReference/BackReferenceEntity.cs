using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ManyToMany.BackReference
{
	[Audited]
	public class BackReferenceEntity
	{
		public virtual long Id { get; set; }

		public virtual OwningEntity Owning { get; set; }
		
		public override bool Equals(object obj)
		{
			if (!(obj is BackReferenceEntity other))
				return false;
			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
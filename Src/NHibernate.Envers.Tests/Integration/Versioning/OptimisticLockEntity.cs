using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Versioning
{
	[Audited]
	public class OptimisticLockEntity
	{
		public virtual int Id { get; set; }
		public virtual string Str { get; set; }
		public virtual int Version { get; protected set; }

		public override bool Equals(object obj)
		{
			var other = obj as OptimisticLockEntity;
			if (other == null)
				return false;
			return Id == other.Id && Str.Equals(other.Str);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Str.GetHashCode();
		}
	}
}
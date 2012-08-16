using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.OneToMany.EmbeddedId
{
	[Audited]
	public class Constant
	{
		public virtual string Id { get; set; }
		public virtual string Name { get; set; }
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Constant)) return false;
			return Equals((Constant) obj);
		}

		public virtual bool Equals(Constant other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Id, Id) && Equals(other.Name, Name);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Id != null ? Id.GetHashCode() : 0)*397) ^ (Name != null ? Name.GetHashCode() : 0);
			}
		}
	}
}
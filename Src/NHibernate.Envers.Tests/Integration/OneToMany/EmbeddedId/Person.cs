using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.OneToMany.EmbeddedId
{
	[Audited]
	public class Person
	{
		public Person()
		{
			PersonATuples = new HashSet<PersonTuple>();
			PersonBTuples = new HashSet<PersonTuple>();
		}

		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<PersonTuple> PersonATuples { get; set; }
		public virtual ISet<PersonTuple> PersonBTuples { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Person)) return false;
			return Equals((Person)obj);
		}

		public virtual bool Equals(Person other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id == Id && Equals(other.Name, Name);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode() : 0);
			}
		}
	}
}
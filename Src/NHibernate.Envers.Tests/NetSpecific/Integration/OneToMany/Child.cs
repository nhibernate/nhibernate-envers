using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToMany
{
	[Audited]
	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Str { get; set; }
		public virtual string SomeFormula { get; protected set; } //NHE-135

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Child)) return false;
			return Equals((Child) obj);
		}

		public virtual bool Equals(Child other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id.Equals(Id) && Equals(other.Str, Str);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id.GetHashCode()*397) ^ (Str != null ? Str.GetHashCode() : 0);
			}
		}
	}
}
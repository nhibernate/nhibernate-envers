using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToMany
{
	[Audited]
	public class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual ISet<Child> Children { get; set; }
		public virtual int Data { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Parent)) return false;
			return Equals((Parent) obj);
		}

		public virtual bool Equals(Parent other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id.Equals(Id) && other.Data == Data;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id.GetHashCode()*397) ^ Data;
			}
		}
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalList.InverseFalse
{
	[Audited]
	public class Parent
	{
		public Parent()
		{
			Children = new List<Child>();
		}

		public virtual int Id { get; set; }
		public virtual IList<Child> Children
		{
			get;
			protected set;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Parent casted))
				return false;
			return (Id == casted.Id);
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
	
	[Audited]
	public class Child
	{
		public virtual int Id { get; set; }
		public virtual Parent Parent { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is Child casted))
				return false;
			return (Id == casted.Id);
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}
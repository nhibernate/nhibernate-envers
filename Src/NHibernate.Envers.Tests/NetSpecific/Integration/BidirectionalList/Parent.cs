using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalList
{
	[Audited]
	public class Parent
	{
		private readonly IList<Child> _children;

		public Parent()
		{
			_children = new List<Child>();
		}

		public virtual int Id { get; set; }
		public virtual IList<Child> Children
		{
			get { return _children; }
		}

		public override bool Equals(object obj)
		{
			var casted = obj as Parent;
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
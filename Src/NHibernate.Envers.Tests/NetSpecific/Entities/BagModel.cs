using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Entities
{
	[Audited]
	public class BagParent
	{
		public BagParent()
		{
			Children = new List<BagChild>();
		}
		public virtual Guid Id { get; set; }
		public virtual IList<BagChild> Children { get; set; }

		public virtual void AddChild(BagChild child)
		{
			child.Parent = this;
			Children.Add(child);
		}
	}

	[Audited]
	public class BagChild
	{
		public virtual Guid Id { get; set; }
		public virtual BagParent Parent { get; set; }
		public virtual string Name { get; set; }
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Access.None.ManyToOne
{
	[Audited]
	public class Parent
	{
		public Parent()
		{
			Children = new HashSet<Child>();
		}

		public virtual int Id { get; set; } 
		public virtual ISet<Child> Children { get; protected set; }

		public virtual void AddChild(string name)
		{
			Children.Add(new Child
			{
				Parent = this,
				Name = name
			});
		}
	}

	[Audited]
	public class Child
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Parent Parent { get; set; }
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.MultiLevelInheritance
{
	[Audited]
	public class Parent
	{
		public virtual int Id { get; set; }
		public virtual ISet<Child> Childs { get; set; }
		public virtual string Property { get; set; }
	}
}
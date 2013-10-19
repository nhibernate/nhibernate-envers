using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Strategy.SetOfValues
{
	[Audited]
	public class SetOfValuesTestEntity
	{
		public SetOfValuesTestEntity()
		{
			ChildValues = new HashSet<string>();
		}

		public virtual int Id { get; set; }
		public virtual ISet<string> ChildValues { get; set; }
	}
}
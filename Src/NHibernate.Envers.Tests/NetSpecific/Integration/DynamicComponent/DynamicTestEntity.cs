using System.Collections;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.DynamicComponent
{
	[Audited]
	public class DynamicTestEntity
	{
		public DynamicTestEntity()
		{
			Properties = new Hashtable();
		}

		public virtual int Id { get; set; }
		public virtual IDictionary Properties { get; set; }
	}
}
using System.Collections.Generic;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model
{
	public class RevisionEntityWithEntityNames
	{
		public virtual long Number { get; set; }
		public virtual long Timestamp { get; set; }
		public virtual ISet<string> EntityNames { get; set; }
	}
}
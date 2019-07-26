using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.TransactionUnexpectedFlush
{
	[Audited]
	public class Entity
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }
	}
}

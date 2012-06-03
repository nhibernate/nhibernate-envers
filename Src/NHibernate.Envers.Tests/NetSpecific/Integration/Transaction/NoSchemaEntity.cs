using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Transaction
{
	[Audited]
	public class NoSchemaEntity
	{
		public virtual Guid Id { get; set; }
	}
}
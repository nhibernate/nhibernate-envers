using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.RevInfo
{
	[RevisionEntity(typeof(string))]
	public class RevEntityIncorrectListenerType
	{
		[RevisionNumber]
		public virtual long Id { get; set; }

		[RevisionTimestamp]
		public virtual DateTime Timestamp { get; set; }
	}
}
using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ModifiedFlags
{
	[Audited]
	public class Casee
	{

		public virtual int Id { get; set; }

		public virtual DateTime? LastModifyDate { get; set; }

		public virtual ISet<CaseToCaseTag> CaseTags { get; set; }

	}

	[Audited]
	public class CaseToCaseTag
	{

		public virtual int Id { get; set; }

		public virtual Casee Right { get; set; }

	}
}

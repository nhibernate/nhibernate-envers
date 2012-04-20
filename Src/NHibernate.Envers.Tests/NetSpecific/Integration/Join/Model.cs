﻿using System;
using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Join
{
	public class NotAudited
	{
		public virtual int Id { get; set; }
	}
	[Audited]
	public class Audited
	{
		public virtual int Id { get; set; }
		[NotAudited]
		public virtual ISet<NotAudited> XCollection { get; set; }

		public virtual int Number { get; set; }
	}
}
﻿using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Collection.NoRevision
{
	[Audited]
	public class Person
	{
		public Person()
		{
			Names = new HashedSet<Name>();
		}

		public virtual int Id { get; set; }
		public virtual ISet<Name> Names { get; set; }
	}
}
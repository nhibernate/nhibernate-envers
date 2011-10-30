﻿using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.EntityNames.OneToManyNotAudited
{
	public class Person
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Age { get; set; }
	}
}
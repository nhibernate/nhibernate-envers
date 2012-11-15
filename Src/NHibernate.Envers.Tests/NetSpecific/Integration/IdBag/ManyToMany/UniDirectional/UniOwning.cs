using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.IdBag.ManyToMany.UniDirectional
{
	[Audited]
	public class UniOwning
	{
		public UniOwning()
		{
			Referencing = new List<UniOwned>();
		}

		public virtual Guid Id { get; set; }
		public virtual ICollection<UniOwned> Referencing { get; set; }
	}
}
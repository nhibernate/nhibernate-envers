using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags.Entities.ManyToMany
{
	[Audited]
	public class Professor
	{
		public Professor()
		{
			Students = new HashSet<Student>();
		}

		public virtual int Id { get; set; }
		public virtual ISet<Student> Students { get; set; }
	}
}
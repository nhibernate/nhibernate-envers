using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags.Entities.ManyToMany
{
	[Audited]
	public class Student
	{
		public Student()
		{
			Professors = new HashSet<Professor>();
		}

		public virtual int Id { get; set; }
		public virtual ISet<Professor> Professors { get; set; }
	}
}
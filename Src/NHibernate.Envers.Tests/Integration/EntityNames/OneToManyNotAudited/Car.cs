using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.EntityNames.OneToManyNotAudited
{
	public class Car
	{
		public virtual long Id { get; set; }
		[Audited]
		public virtual int Number { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual IList<Person> Owners { get; set; }
	}
}
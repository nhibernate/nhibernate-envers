using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.Relation.Unidirectional
{
	[Audited]
	public abstract class AbstractSetEntity
	{
		protected AbstractSetEntity()
		{
			Entities = new HashSet<AbstractContainedEntity>();
		}

		public virtual int Id { get; set; }
		[AuditJoinTable(TableName = "ASE_ACE_AUD")]
		public virtual ISet<AbstractContainedEntity> Entities { get; set; }

	}
}
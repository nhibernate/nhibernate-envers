using Iesi.Collections.Generic;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.Relation.Unidirectional
{
	[Audited]
	public abstract class AbstractSetEntity
	{
		protected AbstractSetEntity()
		{
			Entities = new HashedSet<AbstractContainedEntity>();
		}

		public virtual int Id { get; set; }
		public virtual ISet<AbstractContainedEntity> Entities { get; set; }

	}
}
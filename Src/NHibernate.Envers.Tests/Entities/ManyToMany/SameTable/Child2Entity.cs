using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.ManyToMany.SameTable
{
	[Audited]
	public class Child2Entity
	{
		public virtual int Id { get; set; }
		public virtual IList<ParentEntity> Parents { get; set; }

		public Child2Entity()
		{
			Parents = new List<ParentEntity>();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as Child2Entity;
			if (casted == null)
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}
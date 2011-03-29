using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToMany.SameTable
{
	[Audited]
	public class ParentEntity
	{
		public virtual int Id { get; set; }
		public virtual IList<Child1Entity> Children1 { get; set; }
		public virtual IList<Child2Entity> Children2 { get; set; }

		public ParentEntity()
		{
			Children1 = new List<Child1Entity>();
			Children2 = new List<Child2Entity>();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as ParentEntity;
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
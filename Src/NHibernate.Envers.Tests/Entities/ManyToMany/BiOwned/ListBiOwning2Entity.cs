using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.ManyToMany.BiOwned
{
	[Audited]
	public class ListBiOwning2Entity
	{
		public virtual int Id { get; set; }
		public virtual IList<ListBiOwning1Entity> Referencing { get; set; }

		public ListBiOwning2Entity()
		{
			Referencing = new List<ListBiOwning1Entity>();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as ListBiOwning2Entity;
			if (casted == null)
				return false;
			return (Id == casted.Id);
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}
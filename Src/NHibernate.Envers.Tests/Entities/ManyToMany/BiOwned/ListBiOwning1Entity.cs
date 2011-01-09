using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.ManyToMany.BiOwned
{
	[Audited]
	public class ListBiOwning1Entity
	{
		public virtual int Id { get; set; }
		public virtual IList<ListBiOwning2Entity> Referencing { get; set; }

		public ListBiOwning1Entity()
		{
			Referencing = new List<ListBiOwning2Entity>();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as ListBiOwning1Entity;
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
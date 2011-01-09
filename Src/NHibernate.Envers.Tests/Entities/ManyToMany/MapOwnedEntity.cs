using Iesi.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.ManyToMany
{
	public class MapOwnedEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual ISet<MapOwningEntity> Referencing { get; set; }

		public MapOwnedEntity()
		{
			Referencing = new HashedSet<MapOwningEntity>();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as MapOwnedEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data == casted.Data);
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}
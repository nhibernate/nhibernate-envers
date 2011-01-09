using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.ManyToMany
{
	public class MapOwningEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual IDictionary<string, MapOwnedEntity> References { get; set; }

		public MapOwningEntity()
		{
			References = new Dictionary<string, MapOwnedEntity>();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as MapOwningEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}
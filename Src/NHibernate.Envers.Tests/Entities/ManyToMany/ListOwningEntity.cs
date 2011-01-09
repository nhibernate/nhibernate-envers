using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.ManyToMany
{
	public class ListOwningEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual IList<ListOwnedEntity> References { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ListOwningEntity;
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
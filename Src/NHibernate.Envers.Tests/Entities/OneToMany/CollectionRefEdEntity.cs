using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.OneToMany
{
	public class CollectionRefEdEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual IList<CollectionRefIngEntity> Reffering { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as CollectionRefEdEntity;
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
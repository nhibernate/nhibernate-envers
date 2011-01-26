using System;

namespace NHibernate.Envers.Tests.Entities.OneToMany
{
	[Serializable]
	public class CollectionRefIngEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual CollectionRefEdEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as CollectionRefIngEntity;
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
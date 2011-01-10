using Iesi.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Ids;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Ids
{
	public class SetRefEdEmbIdEntity
	{
		public virtual EmbId Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual ISet<SetRefIngEmbIdEntity> Reffering { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as SetRefEdEmbIdEntity;
			if (casted == null)
				return false;
			return (Id.Equals(casted.Id) && Data == casted.Data);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Data.GetHashCode();
		}
	}
}
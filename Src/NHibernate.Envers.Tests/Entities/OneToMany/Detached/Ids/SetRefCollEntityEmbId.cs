using Iesi.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Ids;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached.Ids
{
	public class SetRefCollEntityEmbId
	{
		public virtual EmbId Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual ISet<EmbIdTestEntity> Collection { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as SetRefCollEntityEmbId;
			if (casted == null)
				return false;
			return (Id.Equals(casted.Id) && Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Data.GetHashCode();
		}
	}
}
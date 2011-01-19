using NHibernate.Envers.Tests.Entities.Ids;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.Ids
{
	public class BiEmbIdRefIngEntity
	{
		public virtual EmbId Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual BiEmbIdRefEdEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as BiEmbIdRefIngEntity;
			if (casted == null)
				return false;
			return Id.Equals(casted.Id) && Data.Equals(casted.Data);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Data.GetHashCode();
		}
	}
}
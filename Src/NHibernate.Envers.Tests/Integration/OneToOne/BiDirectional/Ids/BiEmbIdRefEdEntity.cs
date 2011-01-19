using NHibernate.Envers.Tests.Entities.Ids;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.Ids
{
	public class BiEmbIdRefEdEntity
	{
		public virtual EmbId Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual BiEmbIdRefIngEntity Referencing { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as BiEmbIdRefEdEntity;
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
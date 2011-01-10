using NHibernate.Envers.Tests.Entities.Ids;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Ids
{
	public class SetRefIngEmbIdEntity
	{
		public virtual EmbId Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual SetRefEdEmbIdEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as SetRefIngEmbIdEntity;
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
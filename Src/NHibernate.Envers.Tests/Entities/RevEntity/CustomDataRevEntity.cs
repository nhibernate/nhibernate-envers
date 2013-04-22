using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	[RevisionEntity]
	public class CustomDataRevEntity
	{
		[RevisionNumber]
		public virtual long CustomId { get; set; }

		[RevisionTimestamp]
		public virtual long CustomTimestamp { get; set; }

		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as CustomDataRevEntity;
			if (casted == null)
				return false;
			return CustomId == casted.CustomId &&
					CustomTimestamp.Equals(casted.CustomTimestamp) &&
					(Data == null ? casted.Data == null : Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return CustomId.GetHashCode() ^ CustomTimestamp.GetHashCode() ^ (Data == null ? 0 : Data.GetHashCode());
		}
	}
}
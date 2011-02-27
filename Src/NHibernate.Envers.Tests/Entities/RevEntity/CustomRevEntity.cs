using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	[RevisionEntity]
	public class CustomRevEntity
	{
		private long _customId;
		private long _customTimestamp;

		[RevisionNumber]
		public virtual long CustomId 
		{ 
			get { return _customId; }
			set { _customId = value; }
		}

		[RevisionTimestamp]
		public virtual long CustomTimestamp
		{
			get { return _customTimestamp; }
			set { _customTimestamp = value; }
		}

		public override bool Equals(object obj)
		{
			var casted = obj as CustomRevEntity;
			if (casted == null)
				return false;
			return (CustomId == casted.CustomId && CustomTimestamp == casted.CustomTimestamp);
		}

		public override int GetHashCode()
		{
			return CustomId.GetHashCode() ^ CustomTimestamp.GetHashCode();
		}
	}
}
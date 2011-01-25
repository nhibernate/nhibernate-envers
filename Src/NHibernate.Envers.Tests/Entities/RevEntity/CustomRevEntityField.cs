namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	[RevisionEntity]
	public class CustomRevEntityField
	{
		[RevisionNumber]
		private long _customId;
		[RevisionTimestamp]
		private long _customTimestamp;

		public virtual long CustomId
		{
			get { return _customId; }
			set { _customId = value; }
		}

		public virtual long CustomTimestamp
		{
			get { return _customTimestamp; }
			set { _customTimestamp = value; }
		}

		public override bool Equals(object obj)
		{
			var casted = obj as CustomRevEntityField;
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
namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	[RevisionEntity]
	public class CustomRevEntity
	{
		[RevisionNumber]
		public virtual int CustomId { get; set; }

		[RevisionTimestamp]
		public virtual long CustomTimestamp { get; set; }

		public override bool Equals(object obj)
        {
            var casted = obj as CustomRevEntity;
            if (casted == null)
                return false;
            return (CustomId == casted.CustomId && CustomTimestamp == casted.CustomTimestamp);
        }

        public override int GetHashCode()
        {
            return CustomId ^ CustomTimestamp.GetHashCode();
        }
    }
}
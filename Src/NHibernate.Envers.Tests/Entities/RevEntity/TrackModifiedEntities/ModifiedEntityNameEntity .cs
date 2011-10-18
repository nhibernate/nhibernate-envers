namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	/// <summary>
	/// Custom detail of revision entity.
	/// </summary>
	public class ModifiedEntityTypeEntity 
	{
		public virtual int Id { get; set; }
		public virtual CustomTrackingRevisionEntity Revision { get; set; }
		public virtual string EntityName { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ModifiedEntityTypeEntity;
			if (casted == null)
				return false;
			return EntityName == casted.EntityName;
		}

		public override int GetHashCode()
		{
			return EntityName.GetHashCode();
		}
	}
}
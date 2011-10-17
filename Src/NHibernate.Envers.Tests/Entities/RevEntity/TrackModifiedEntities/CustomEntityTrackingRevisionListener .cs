namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	public class CustomEntityTrackingRevisionListener : IEntityTrackingRevisionListener 
	{
		public void NewRevision(object revisionEntity)
		{
		}

		public void AddEntityToRevision(string entityName, object revisionEntity)
		{
			((CustomTrackingRevisionEntity) revisionEntity).AddModifiedEntityName(entityName);
		}

		public void RemoveEntityFromRevision(string entityName, object revisionEntity)
		{
			((CustomTrackingRevisionEntity)revisionEntity).RemoveModifiedEntityName(entityName);
		}
	}
}
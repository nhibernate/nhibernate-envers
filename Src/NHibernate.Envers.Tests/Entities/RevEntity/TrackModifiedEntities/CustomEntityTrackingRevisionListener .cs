namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	public class CustomTrackingRevisionListener : IEntityTrackingRevisionListener 
	{
		public void NewRevision(object revisionEntity)
		{
		}

		public void EntityChanged(System.Type entityClass, string entityName, object entityId, RevisionType revisionType, object revisionEntity)
		{
			((CustomTrackingRevisionEntity)revisionEntity).AddModifiedEntityType(entityClass.FullName);
		}
	}
}
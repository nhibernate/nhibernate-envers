namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	public class CustomEntityTrackingRevisionListener : IEntityTrackingRevisionListener 
	{
		public void NewRevision(object revisionEntity)
		{
		}

		public void EntityChanged(System.Type entityClass, string entityName, object entityId, RevisionType revisionType, object revisionEntity)
		{
			((CustomTrackingRevisionEntity)revisionEntity).AddModifiedEntityName(entityClass.FullName);
		}
	}
}
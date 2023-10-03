namespace NHibernate.Envers.RevisionInfo
{
	public partial interface IRevisionInfoGenerator
	{
		void SaveRevisionData(ISession session, object revisionData);
		object Generate();

		/// <see cref="IEntityTrackingRevisionListener.EntityChanged"/>
		void EntityChanged(System.Type entityClass, string entityName, object entityId, RevisionType revisionType, object revisionEntity);
	}
}

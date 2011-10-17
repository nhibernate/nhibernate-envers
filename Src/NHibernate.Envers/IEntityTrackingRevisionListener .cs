namespace NHibernate.Envers
{
	public interface IEntityTrackingRevisionListener : IRevisionListener
	{
		void AddEntityToRevision(string entityName, object revisionEntity);
		void RemoveEntityFromRevision(string entityName, object revisionEntity);
	}
}
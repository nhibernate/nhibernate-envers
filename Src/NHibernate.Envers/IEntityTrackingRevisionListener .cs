using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers
{
	/// <summary>
	/// Extension of standard <see cref="IRevisionListener"/> that notifies whenever an entity instance has been
	/// added, modified or removed within current revision boundaries.
	/// </summary>
	public interface IEntityTrackingRevisionListener : IRevisionListener
	{
		/// <summary>
		/// Called after audited entity data has been persisted.
		/// </summary>
		/// <param name="entityClass">Audited entity class.</param>
		/// <param name="entityName">
		/// Name of the audited entity. May be useful when Java class is mapped multiple times,
		/// potentially to different tables. 
		/// </param>
		/// <param name="entityId">Identifier of modified entity.</param>
		/// <param name="revisionType">Modification type (addition, update or removal).</param>
		/// <param name="revisionEntity">An instance of the entity annotated with <see cref="RevisionEntityAttribute"/>.</param>
		void EntityChanged(System.Type entityClass, string entityName, object entityId, RevisionType revisionType, object revisionEntity);
	}
}
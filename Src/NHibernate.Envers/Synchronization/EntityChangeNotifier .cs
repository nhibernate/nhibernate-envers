using NHibernate.Engine;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Synchronization.Work;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Synchronization
{
	/// <summary>
	/// Notifies <see cref="IRevisionInfoGenerator"/> about changes made in the current revision.
	/// </summary>
	public class EntityChangeNotifier 
	{
		private readonly IRevisionInfoGenerator _revisionInfoGenerator;
		private readonly ISessionImplementor _sessionImplementor;

		public EntityChangeNotifier(IRevisionInfoGenerator revisionInfoGenerator, ISessionImplementor sessionImplementor)
		{
			_revisionInfoGenerator = revisionInfoGenerator;
			_sessionImplementor = sessionImplementor;
		}

		/// <summary>
		/// Notifies <see cref="IRevisionInfoGenerator"/> about changes made in the current revision. Provides information
		/// about modified entity class, entity name and its id, as well as <see cref="RevisionType"/> and revision log entity.
		/// </summary>
		/// <param name="session">Active session.</param>
		/// <param name="currentRevisionData">Revision log entity.</param>
		/// <param name="vwu">Performed work unit.</param>
		public void EntityChanged(ISession session, object currentRevisionData, IAuditWorkUnit vwu)
		{
			var entityId = vwu.EntityId;
			var idAsPersistentColl = entityId as PersistentCollectionChangeWorkUnit.PersistentCollectionChangeWorkUnitId;
			if (idAsPersistentColl != null)
			{
				entityId = idAsPersistentColl.OwnerId;
			}
			var entClass = Toolz.ResolveEntityClass(_sessionImplementor, vwu.EntityName);
			_revisionInfoGenerator.EntityChanged(entClass, vwu.EntityName, entityId, vwu.RevisionType, currentRevisionData);
		}
	}
}
using System;
using System.Collections;
using NHibernate.Envers.Query;

namespace NHibernate.Envers
{
	public interface IAuditReader 
	{
		/// <summary>
		/// Find an entity by primary key at the given revision.
		/// </summary>
		/// <typeparam name="T">Type of entity</typeparam>
		/// <param name="primaryKey">Primary key of the entity.</param>
		/// <param name="revision">Revision in which to get the entity</param>
		/// <returns>
		/// The found entity instance at the given revision (its properties may be partially filled
		/// if not all properties are audited) or null, if an entity with that id didn't exist at that
		/// revision.
		/// </returns>
		T Find<T>(object primaryKey, long revision);

		/// <summary>
		/// Find an entity by primary key at the given revision.
		/// </summary>
		/// <param name="cls">Type of entity</param>
		/// <param name="primaryKey">Primary key of the entity.</param>
		/// <param name="revision">Revision in which to get the entity</param>
		/// <returns>
		/// The found entity instance at the given revision (its properties may be partially filled
		/// if not all properties are audited) or null, if an entity with that id didn't exist at that
		/// revision.
		/// </returns>
		object Find(System.Type cls, object primaryKey, long revision);

		/// <summary>
		/// Get a list of revision numbers, at which an entity was modified.
		/// </summary>
		/// <param name="cls">Class of the entity.</param>
		/// <param name="primaryKey">Primary key of the entity.</param>
		/// <returns>
		/// A list of revision numbers, at which the entity was modified, sorted in ascending order (so older
		/// revisions come first).
		/// </returns>
		IList GetRevisions(System.Type cls, object primaryKey);

		/// <summary>
		/// Get the date, at which a revision was created. 
		/// </summary>
		/// <param name="revision">Number of the revision for which to get the date.</param>
		/// <returns>Date of commiting the given revision.</returns>
		DateTime GetRevisionDate(long revision);

		/// <summary>
		/// Gets the revision number, that corresponds to the given date. More precisely, returns
		/// the number of the highest revision, which was created on or before the given date. So:
		/// <code>getRevisionDate(GetRevisionNumberForDate(date)) <= date</code> and
		/// <code>getRevisionDate(GetRevisionNumberForDate(date)+1) > date</code>.
		/// </summary>
		/// <param name="date">Date for which to get the revision.</param>
		/// <returns></returns>
		long GetRevisionNumberForDate(DateTime date);

		/// <summary>
		/// A helper method; should be used only if a custom revision entity is used.
		/// </summary>
		/// <typeparam name="T">Class of the revision entity. Should be annotated with RevisionEntity.</typeparam>
		/// <param name="revision">Number of the revision for which to get the data.</param>
		/// <returns>Entity containing data for the given revision.</returns>
		T FindRevision<T>(long revision);

		/// <summary>
		/// A helper method; should be used only if a custom revision entity is used.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type">Class of the revision entity. Should be annotated with RevisionEntity.</param>
		/// <param name="revision">Number of the revision for which to get the data.</param>
		/// <returns>Entity containing data for the given revision.</returns>
		object FindRevision(System.Type type, long revision);

		/// <summary>
		/// Gets an instance of the current revision entity, to which any entries in the audit tables will be bound.
		/// Please note the if {@code persist} is {@code false}, and no audited entities are modified in this session,
		/// then the obtained revision entity instance won't be persisted. If {@code persist} is {@code true}, the revision
		/// entity instance will always be persisted, regardless of whether audited entities are changed or not.
		/// </summary>
		/// <typeparam name="T">Class of the revision entity. Should be annotated with {@link RevisionEntity}.</typeparam>
		/// <param name="persist">
		/// If the revision entity is not yet persisted, should it become persisted. This way, the primary
		/// identifier (id) will be filled (if it's assigned by the DB) and available, but the revision entity will be
		/// persisted even if there are no changes to audited entities. Otherwise, the revision number (id) can be
		/// null.</param>
		/// <returns>The current revision entity, to which any entries in the audit tables will be bound.</returns>
		T GetCurrentRevision<T>(bool persist);

		/// <summary>
		/// Gets an instance of the current revision entity, to which any entries in the audit tables will be bound.
		/// Please note the if {@code persist} is {@code false}, and no audited entities are modified in this session,
		/// then the obtained revision entity instance won't be persisted. If {@code persist} is {@code true}, the revision
		/// entity instance will always be persisted, regardless of whether audited entities are changed or not.
		/// </summary>
		/// <param name="type">Class of the revision entity. Should be annotated with {@link RevisionEntity}.</param>
		/// <param name="persist">
		/// If the revision entity is not yet persisted, should it become persisted. This way, the primary
		/// identifier (id) will be filled (if it's assigned by the DB) and available, but the revision entity will be
		/// persisted even if there are no changes to audited entities. Otherwise, the revision number (id) can be
		/// null.</param>
		/// <returns>The current revision entity, to which any entries in the audit tables will be bound.</returns>
		object GetCurrentRevision(System.Type type, bool persist);

		/// <summary>
		/// Creates a query
		/// </summary>
		/// <returns>
		/// A query creator, associated with this AuditReader instance, with which queries can be
		/// created and later executed. Shouldn't be used after the associated Session or EntityManager
		/// is closed.
		/// </returns>
		AuditQueryCreator CreateQuery();
	}
}
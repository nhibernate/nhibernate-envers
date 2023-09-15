using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query;

namespace NHibernate.Envers
{
	public partial interface IAuditReader
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
		/// Find an entity by primary key at the given revision with the specified entityName.
		/// </summary>
		/// <param name="entityName">Name of the entity (if can't be guessed basing on the {@code cls}).</param>
		/// <param name="primaryKey">Primary key of the entity.</param>
		/// <param name="revision">Revision in which to get the entity</param>
		/// <returns>
		/// The found entity instance at the given revision (its properties may be partially filled
		/// if not all properties are audited) or null, if an entity with that id didn't exist at that
		/// revision.
		/// </returns>
		object Find(string entityName, object primaryKey, long revision);

		/// <summary>
		/// Find an entity by primary key at the given revision with the specified entityName,
		/// possibly including deleted entities in the search.
		/// </summary>
		/// <param name="entityName">Name of the entity (if can't be guessed basing on the {@code cls}).</param>
		/// <param name="primaryKey">Primary key of the entity.</param>
		/// <param name="revision">Revision in which to get the entity</param>
		/// <param name="includeDeletions">Whether to include deleted entities in the search.</param>
		/// <returns>
		/// The found entity instance at the given revision (its properties may be partially filled
		/// if not all properties are audited) or null, if an entity with that id didn't exist at that
		/// revision.
		/// </returns>
		object Find(string entityName, object primaryKey, long revision, bool includeDeletions);

		/// <summary>
		/// Get a list of revision numbers, at which an entity was modified.
		/// </summary>
		/// <param name="cls">Type of entity</param>
		/// <param name="primaryKey">Primary key of the entity.</param>
		/// <returns>
		/// A list of revision numbers, at which the entity was modified, sorted in ascending order (so older
		/// revisions come first).
		/// </returns>
		IEnumerable<long> GetRevisions(System.Type cls, object primaryKey);

		/// <summary>
		/// Get a list of revision numbers, at which an entity was modified, looking by entityName.
		/// </summary>
		/// <param name="entityName">Name of the entity </param>
		/// <param name="primaryKey">Primary key of the entity.</param>
		/// <returns>
		/// A list of revision numbers, at which the entity was modified, sorted in ascending order (so older
		/// revisions come first).
		/// </returns>
		IEnumerable<long> GetRevisions(string entityName, object primaryKey);

		/// <summary>
		/// Get the date, at which a revision was created. 
		/// </summary>
		/// <param name="revision">Number of the revision for which to get the date.</param>
		/// <returns>Date of commiting the given revision.</returns>
		DateTime GetRevisionDate(long revision);

		/// <summary>
		/// Gets the revision number, that corresponds to the given date.
		/// </summary>
		/// <param name="date">Date for which to get the revision.</param>
		/// <returns>The number of the highest revision, which was created on or before the given <paramref name="date"/>.</returns>
		/// <remarks>
		/// The result is that:
		/// <code>
		/// <![CDATA[
		/// GetRevisionDate(GetRevisionNumberForDate(date)) <= date
		/// ]]>
		/// </code>
		/// and
		/// <code>
		/// <![CDATA[
		/// GetRevisionDate(GetRevisionNumberForDate(date)+1) > date
		/// ]]>
		/// </code>
		/// </remarks>
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
		/// <param name="revision">Number of the revision for which to get the data.</param>
		/// <returns>Entity containing data for the given revision.</returns>
		object FindRevision(long revision);

		/// <summary>
		/// Find a map of revisions using the revision numbers specified.
		/// </summary>
		/// <param name="revisions">Revision numbers of the revision for which to get the data.</param>
		/// <returns>A map of revision number and the given revision entity.</returns>
		IDictionary<long, object> FindRevisions(IEnumerable<long> revisions);

		/// <summary>
		/// Find a map of revisions using the revision numbers specified.
		/// </summary>
		/// <typeparam name="T">The revision type user has defined</typeparam>
		/// <param name="revisions">Revision numbers of the revision for which to get the data.</param>
		/// <returns>A map of revision number and the given revision entity.</returns>
		IDictionary<long, T> FindRevisions<T>(IEnumerable<long> revisions);

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
		/// <param name="persist">
		/// If the revision entity is not yet persisted, should it become persisted. This way, the primary
		/// identifier (id) will be filled (if it's assigned by the DB) and available, but the revision entity will be
		/// persisted even if there are no changes to audited entities. Otherwise, the revision number (id) can be
		/// null.</param>
		/// <returns>The current revision entity, to which any entries in the audit tables will be bound.</returns>
		object GetCurrentRevision(bool persist);

		/// <summary>
		/// Creates a query
		/// </summary>
		/// <returns>
		/// A query creator, associated with this AuditReader instance, with which queries can be
		/// created and later executed. Shouldn't be used after the associated Session or EntityManager
		/// is closed.
		/// </returns>
		AuditQueryCreator CreateQuery();

		/// <returns>
		/// 	Basic implementation of &amp;lt;see cref=&amp;quot;ICrossTypeRevisionChangesReader&amp;quot;/&amp;gt; interface. Raises an exception if the default
		/// 	mechanism of tracking entity names modified during revisions has not been enabled.
		/// </returns>
		///<exception cref="AuditException">
		/// If none of the following conditions is satisfied:
		/// <ul>
		///	<li><code>nhibernate.envers.track_entities_changed_in_revision</code>
		///   parameter is set to <code>true</code>.</li>
		///   <li>Custom revision entity (annotated with <see cref="RevisionEntityAttribute"/>)
		///	extends <see cref="DefaultTrackingModifiedEntitiesRevisionEntity"/> base class.</li>
		///   <li>Custom revision entity (annotated with <see cref="RevisionEntityAttribute"/>) encapsulates a field
		///   marked with <see cref="ModifiedEntityNamesAttribute"/> attribute.</li>
		/// </ul>
		/// </exception>
		ICrossTypeRevisionChangesReader CrossTypeRevisionChangesReader();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="primaryKey"></param>
		/// <param name="revision"></param>
		/// <param name="entity">entity that was obtained previously from the same AuditReader.</param>
		/// <returns>
		/// the entityName for the given entity, null in case the entity is
		/// not associated with this AuditReader instance.
		/// </returns>
		string GetEntityName(object primaryKey, long revision, object entity);

		/// <summary>
		/// Checks if the entityClass was configured to be audited.
		/// </summary>
		/// <param name="entityClass">Class of the entity asking for audit support</param>
		/// <returns>true if the entityClass is audited.</returns>
		bool IsEntityClassAudited(System.Type entityClass);

		/// <summary>
		/// Checks if the entityName was configured to be audited.
		/// </summary>
		/// <param name="entityName">EntityName of the entity asking for audit support.</param>
		/// <returns>true if the entityName is audited.</returns>
		bool IsEntityNameAudited(string entityName);
	}
}
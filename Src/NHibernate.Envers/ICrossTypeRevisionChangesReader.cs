using System;
using System.Collections.Generic;

namespace NHibernate.Envers
{
	/// <summary>
	/// Queries that allow retrieving snapshots of all entities (regardless of their particular type) changed in the given
	/// revision. Note that this API can be legally used only when default mechanism of tracking modified entity names
	/// is enabled.
	/// </summary>
	public partial interface ICrossTypeRevisionChangesReader
	{
		/// <summary>
		/// Returns set of entity classes modified in a given revision.
		/// </summary>
		/// <param name="revision">Revision number.</param>
		/// <returns>Set of classes modified in a given revision.</returns>
		ISet<Tuple<string, System.Type>> FindEntityTypes(long revision);

		/// <summary>
		/// Find all entities changed (added, updated and removed) in a given revision. Executes <i>n+1</i> SQL queries,
		/// where <i>n</i> is a number of different entity classes modified within specified revision.
		/// </summary>
		/// <param name="revision">Revision number.</param>
		/// <returns>Snapshots of all audited entities changed in a given revision.</returns>
		IEnumerable<object> FindEntities(long revision);

		/// <summary>
		/// Find all entities changed (added, updated and removed) in a given revision. Executes <i>n+1</i> SQL queries,
		/// where <i>n</i> is a number of different entity classes modified within specified revision.
		/// </summary>
		/// <param name="revision">Revision number.</param>
		/// <param name="revisionType">Type of modification</param>
		/// <returns>Snapshots of all audited entities changed in a given revision and filtered by modification type.</returns>
		IEnumerable<object> FindEntities(long revision, RevisionType revisionType);

		/// <summary>
		/// Find all entities changed (added, updated and removed) in a given revision grouped by modification type.
		/// Executes <i>mn+1</i> SQL queries, where:
		/// <ul>
		/// <li><i>n</i> - number of different entity classes modified within specified revision.</li>
		/// <li><i>m</i> - number of different revision types. See <see cref="RevisionType"/> enum.</li>
		/// </ul>
		/// </summary>
		/// <param name="revision">Revision number.</param>
		/// <returns>Map containing lists of entity snapshots grouped by modification operation (e.g. addition, update, removal).</returns>
		IDictionary<RevisionType, IEnumerable<object>> FindEntitiesGroupByRevisionType(long revision);
	}
}
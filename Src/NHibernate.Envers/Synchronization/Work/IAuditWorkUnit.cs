using System.Collections.Generic;

namespace NHibernate.Envers.Synchronization.Work
{
	public partial interface IAuditWorkUnit: IWorkUnitMergeVisitor, IWorkUnitMergeDispatcher 
	{
		object EntityId { get; }
		string EntityName { get; }
	
		bool ContainsWork();

		bool IsPerformed();

		/// <summary>
		/// Perform this work unit in the given session.
		/// </summary>
		/// <param name="session">Session, in which the work unit should be performed.</param>
		/// <param name="revisionData">
		/// The current revision data, which will be used to populate the work unit with the correct revision relation.
		/// </param>
		void Perform(ISession session, object revisionData);

		void Undo(ISession session);

		/// <summary>
		/// </summary>
		/// <param name="revisionData">The current revision data, which will be used to populate the work unit with the correct revision relation.</param>
		/// <returns>Generates data that should be saved when performing this work unit.</returns>
		IDictionary<string, object> GenerateData(object revisionData);

		/// <summary>
		/// Performed modification type.
		/// </summary>
		RevisionType RevisionType { get; }
	}
}

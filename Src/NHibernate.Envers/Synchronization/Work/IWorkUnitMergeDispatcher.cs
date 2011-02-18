namespace NHibernate.Envers.Synchronization.Work
{
	/// <summary>
	/// Visitor patter dispatcher.
	/// </summary>
	public interface IWorkUnitMergeDispatcher
	{
		/// <summary>
		/// Shuold be invoked on the second work unit.
		/// </summary>
		/// <param name="first">First work unit (that is, the one added earlier).</param>
		/// <returns>The work unit that is the result of the merge.</returns>
		IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first);
	}
}

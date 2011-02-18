namespace NHibernate.Envers.Synchronization.Work
{
	/// <summary>
	/// Visitor pattern visitor. All methods should be invoked on the first work unit.
	/// </summary>
	public interface IWorkUnitMergeVisitor
	{
		IAuditWorkUnit Merge(AddWorkUnit second);
		IAuditWorkUnit Merge(ModWorkUnit second);
		IAuditWorkUnit Merge(DelWorkUnit second);
		IAuditWorkUnit Merge(CollectionChangeWorkUnit second);
		IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second);
	}
}

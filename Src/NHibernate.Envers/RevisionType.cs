namespace NHibernate.Envers
{
	/// <summary>
	/// Type of the revision.
	/// </summary>
	public enum RevisionType
	{
		/// <summary>
		/// Indicates that the entity was added (persisted) at that revision.
		/// </summary>
		Added = 0,

		/// <summary>
		/// Indicates that the entity was modified (one or more of its fields) at that revision.
		/// </summary>
		Modified = 1,

		/// <summary>
		/// Indicates that the entity was deleted (removed) at that revision.
		/// </summary>
		Deleted = 2
	}
}

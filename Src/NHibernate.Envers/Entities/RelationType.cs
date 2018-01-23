namespace NHibernate.Envers.Entities
{
	/// <summary>
	/// Type of a relation between two entities.
	/// </summary>
	public enum RelationType
	{
		/// <summary>
		/// A single-reference-valued relation. The entity owns the relation.
		/// </summary>
		ToOne,

		/// <summary>
		/// A single-reference-valued relation. The entity doesn't own the relation. It is directly mapped in the related entity.
		/// </summary>
		ToOneNotOwning,

		/// <summary>
		/// A collection-of-references-valued relation. The entity doesn't own the relation. It is directly mapped in the related entity.
		/// </summary>
		ToManyNotOwning,

		/// <summary>
		/// A collection-of-references-valued relation. The entity owns the relation. It is mapped using a middle table.
		/// </summary>
		ToManyMiddle,

		/// <summary>
		/// A collection-of-references-valued relation. The entity doesn't own the relation. It is mapped using a middle table.
		/// </summary>
		ToManyMiddleNotOwning
	}
}
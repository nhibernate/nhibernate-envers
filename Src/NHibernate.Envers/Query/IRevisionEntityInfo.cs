namespace NHibernate.Envers.Query
{
	/// <summary>
	/// Detailed result of a query over the history of an entity. 
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TRevisionEntity">The type of RevisionEntity</typeparam>
	/// <seealso cref="DefaultRevisionEntity"/>
	public interface IRevisionEntityInfo<out TEntity, out TRevisionEntity>
	{
		TEntity Entity { get; }
		TRevisionEntity RevisionEntity { get; }
		RevisionType Operation { get; }
	}
}
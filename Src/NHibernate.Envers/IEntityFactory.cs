namespace NHibernate.Envers
{
	/// <summary>
	/// Contract for instantiation of an entity
	/// </summary>
	public interface IEntityFactory
	{
		object Instantiate(System.Type type);
	}

	/// <summary>
	/// Strongly-typed version of IEntityFactory
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IEntityFactory<TEntity> : IEntityFactory
	{
		TEntity Instantiate();
	}
}
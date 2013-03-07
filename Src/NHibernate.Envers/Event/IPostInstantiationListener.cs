namespace NHibernate.Envers.Event
{
	/// <summary>
	/// Called after a versioned entity is instantiated
	/// Users can implement their own by setting <see cref="NHibernate.Envers.Configuration.ConfigurationKey.PostInstantiationListener"/>.
	/// </summary>
	/// <remarks>
	/// Implementations of this interface must have a public, default ctor
	/// </remarks>
	public interface IPostInstantiationListener
	{
		/// <summary>
		/// Perform any business logic required after a versioned entity has been instantiated
		/// </summary>
		/// <param name="entity">Newly instantiated versioned entity</param>
		void PostInstantiate(object entity);
	}
}
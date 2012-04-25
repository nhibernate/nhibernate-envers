using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	/// <summary>
	/// Creates collection proxies. 
	/// Users can implement their own by setting <see cref="ConfigurationKey.CollectionProxyFactory"/>.
	/// </summary>
	/// <remarks>
	/// Implementations of this interface must have a public, default ctor
	/// </remarks>
	public interface ICollectionProxyFactory
	{
		object Create(System.Type collectionInterface, IInitializor collectionInitializor);
	}
}
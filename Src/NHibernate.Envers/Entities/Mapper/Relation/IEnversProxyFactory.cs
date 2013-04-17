using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	/// <summary>
	/// Creates proxies used by Envers. 
	/// Users can implement their own by setting <see cref="ConfigurationKey.ProxyFactory"/>.
	/// </summary>
	/// <remarks>
	/// Implementations of this interface must have a public, default ctor
	/// </remarks>
	public interface IEnversProxyFactory
	{
		/// <summary>
		/// Creates a collection proxy.
		/// </summary>
		object CreateCollectionProxy(System.Type collectionInterface, IInitializor collectionInitializor);

		/// <summary>
		/// Creates a proxy for a x-to-one releationship.
		/// </summary>
		object CreateToOneProxy(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, string referencedEntityName, object entityId, long revision, bool removed);
	}
}
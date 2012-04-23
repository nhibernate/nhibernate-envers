using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	public class CollectionProxyFactory : ICollectionProxyFactory
	{
		private readonly ProxyFactory _proxyFactory;

		public CollectionProxyFactory()
		{
			_proxyFactory = new ProxyFactory();
		}

		public object Create(System.Type collectionInterface, IInitializor collectionInitializor)
		{
			return _proxyFactory.CreateProxy(collectionInterface, new CollectionProxyInterceptor(collectionInitializor));
		}
	}
}
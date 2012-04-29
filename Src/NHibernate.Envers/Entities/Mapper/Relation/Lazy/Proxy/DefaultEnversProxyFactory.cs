using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	public class DefaultEnversProxyFactory : IEnversProxyFactory
	{
		private readonly ProxyFactory _proxyFactory;

		public DefaultEnversProxyFactory()
		{
			_proxyFactory = new ProxyFactory();
		}

		public object CreateCollectionProxy(System.Type collectionInterface, IInitializor collectionInitializor)
		{
			return _proxyFactory.CreateProxy(collectionInterface, new CollectionProxyInterceptor(collectionInitializor));
		}

		public object CreateToOneProxy(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, 
											string referencedEntityName, object entityId, long revision)
		{
			var innerProxy = (ISessionImplementor)_proxyFactory
					.CreateProxy(typeof(ISessionImplementor), new SessionImplToOneInterceptor(versionsReader, entityId, revision, verCfg));

			return versionsReader.SessionImplementor.Factory.GetEntityPersister(referencedEntityName)
					.CreateProxy(entityId, innerProxy);
		}
	}
}
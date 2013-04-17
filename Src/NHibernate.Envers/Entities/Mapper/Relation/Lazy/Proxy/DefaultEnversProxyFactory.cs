using System;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class DefaultEnversProxyFactory : IEnversProxyFactory
	{
		[NonSerialized]
		private ProxyFactory _proxyFactory;

		private ProxyFactory proxyFactory
		{
			get { return _proxyFactory ?? (_proxyFactory = new ProxyFactory()); }
		}

		public object CreateCollectionProxy(System.Type collectionInterface, IInitializor collectionInitializor)
		{
			return proxyFactory.CreateProxy(collectionInterface, new CollectionProxyInterceptor(collectionInitializor));
		}

		public object CreateToOneProxy(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, 
											string referencedEntityName, object entityId, long revision, bool removed)
		{
			var innerProxy = (ISessionImplementor)proxyFactory
					.CreateProxy(typeof(ISessionImplementor), new SessionImplToOneInterceptor(versionsReader, entityId, revision, removed, verCfg));

			return versionsReader.SessionImplementor.Factory.GetEntityPersister(referencedEntityName)
					.CreateProxy(entityId, innerProxy);
		}
	}
}
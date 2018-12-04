using System;
using NHibernate.Collection;
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

		private ProxyFactory proxyFactory => _proxyFactory ?? (_proxyFactory = new ProxyFactory());

		public object CreateCollectionProxy(System.Type collectionInterface, IInitializor collectionInitializor)
		{
			return proxyFactory.CreateProxy(collectionInterface, new CollectionProxyInterceptor(collectionInitializor), typeof(ILazyInitializedCollection));
		}
	}
}
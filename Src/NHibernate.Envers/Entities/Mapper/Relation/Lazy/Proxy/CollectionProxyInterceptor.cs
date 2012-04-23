using System;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class CollectionProxyInterceptor : NHibernate.Proxy.DynamicProxy.IInterceptor
	{
		[NonSerialized]
		private readonly IInitializor _initializor;
		private object _target;

		public CollectionProxyInterceptor(IInitializor initializor)
		{
			_initializor = initializor;
		}

		public object Intercept(InvocationInfo info)
		{
			return info.TargetMethod.Invoke(getOrInitTarget(), info.Arguments);
		}

		private object getOrInitTarget()
		{
			return _target ?? (_target = _initializor.Initialize());
		}
	}
}
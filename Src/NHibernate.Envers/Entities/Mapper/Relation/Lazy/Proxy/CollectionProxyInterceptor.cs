using System;
using System.Reflection;
using NHibernate.Collection;
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

		private static readonly MethodInfo forceInitMethod = typeof(ILazyInitializedCollection).GetMethod("ForceInitialization");
		private static readonly MethodInfo wasInitializedMethod = typeof(ILazyInitializedCollection).GetProperty("WasInitialized").GetMethod;


		public CollectionProxyInterceptor(IInitializor initializor)
		{
			_initializor = initializor;
		}

		public object Intercept(InvocationInfo info)
		{
			// the following methods of ILazyInitializedCollection are implemented to support force initialization using EnversUtil.Initialize 
			if (info.TargetMethod == forceInitMethod)
			{
				getOrInitTarget();
				return null;
			}
			if (info.TargetMethod == wasInitializedMethod)
			{
				return _target != null;
			}

			return info.TargetMethod.Invoke(getOrInitTarget(), info.Arguments);
		}

		private object getOrInitTarget()
		{
			return _target ?? (_target = _initializor.Initialize());
		}
	}
}
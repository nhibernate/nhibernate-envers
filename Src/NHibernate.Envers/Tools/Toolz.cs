using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Util;

namespace NHibernate.Envers.Tools
{
	public static class Toolz 
	{
		public static bool EntitiesEqual(ISessionImplementor session, object obj1, object obj2) 
		{
			var id1 = getIdentifier(session, obj1);
			var id2 = getIdentifier(session, obj2);

			return ObjectsEqual(id1, id2);
		}

		private static object getIdentifier(ISessionImplementor session, object obj)
		{
			return obj == null
				? null
				: obj is INHibernateProxy objAsProxy
					? objAsProxy.HibernateLazyInitializer.Identifier
					: session.GetEntityPersister(session.BestGuessEntityName(obj), obj).GetIdentifier(obj);
		}

		public static object GetTargetFromProxy(ISessionImplementor session, INHibernateProxy proxy) 
		{
			if (!proxy.HibernateLazyInitializer.IsUninitialized) 
				return proxy.HibernateLazyInitializer.GetImplementation();

			var lazyInitializer = proxy.HibernateLazyInitializer;
			var proxySession = (ISession)lazyInitializer.Session;

			var tempSession = proxySession == null
				? CreateChildSession((ISession) session)
				: CreateChildSession(proxySession);
			return tempSession.Get(lazyInitializer.EntityName, lazyInitializer.Identifier);
		}

		public static bool ObjectsEqual(object obj1, object obj2)
		{
			if (obj1 == null) 
			{
				return obj2 == null;
			}

			return obj1.Equals(obj2);
		}

		/**
		 * Transforms a list of arbitrary elements to a list of index-element pairs.
		 * @param list List to transform.
		 * @return A list of pairs: ((0, element_at_index_0), (1, element_at_index_1), ...)
		 */
		public static IList<Tuple<int, object>> ListToIndexElementPairList(IList list)
		{
			var ret = new List<Tuple<int, object>>();

			for (var i = 0; i < list.Count; i++)
			{
				ret.Add(new Tuple<int,object>(i, list[i]));
			}

			return ret;
		}

		public static System.Type ResolveDotnetType(string className)
		{
			return ReflectHelper.ClassForFullName(className);
		}

		public static System.Type ResolveEntityClass(ISessionImplementor sessionImplementor, string entityName)
		{
			var entityPersister = sessionImplementor.Factory.GetEntityPersister(entityName);
			return entityPersister.MappedClass;
		}

		public static bool ArraysEqual(object[] array1, object[] array2)
		{
			if (array1 == null) 
				return array2 == null;
			if (array2 == null || array1.Length != array2.Length) 
				return false;
			for (var i = 0; i < array1.Length; ++i)
			{
				if (array1[i] == null ? array2[i] != null : !array1[i].Equals(array2[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static ISession CreateChildSession(ISession session)
		{
			return session.SessionWithOptions().Connection().ConnectionReleaseMode().FlushMode().Interceptor().OpenSession();
		}
	}
}

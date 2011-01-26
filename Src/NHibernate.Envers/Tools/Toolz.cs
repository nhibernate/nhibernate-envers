using System.Collections.Generic;
using NHibernate.Proxy;
using NHibernate.Engine;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Envers.Tools
{
	public static class Toolz 
	{
		public static bool EntitiesEqual(ISessionImplementor session, object obj1, object obj2) 
		{
			var id1 = GetIdentifier(session, obj1);
			var id2 = GetIdentifier(session, obj2);

			return ObjectsEqual(id1, id2);
		}

		private static object GetIdentifier(ISessionImplementor session, object obj) 
		{
			if (obj == null) 
			{
				return null;
			}

			var objAsProxy = obj as INHibernateProxy;

			return objAsProxy!=null ? objAsProxy.HibernateLazyInitializer.Identifier : session.GetEntityPersister(null, obj).GetIdentifier(obj, session.EntityMode);
		}

		public static object GetTargetFromProxy(ISessionFactoryImplementor sessionFactoryImplementor, INHibernateProxy proxy) 
		{
			if (!proxy.HibernateLazyInitializer.IsUninitialized) 
			{
				return proxy.HibernateLazyInitializer.GetImplementation();
			}

			var sessionImplementor = proxy.HibernateLazyInitializer.Session;
			//ORIG: ISession tempSession = sessionImplementor==null ? sessionFactoryImplementor.openTemporarySession() : sessionImplementor.Factory.openTemporarySession();
			var tempSession = sessionImplementor == null ? sessionFactoryImplementor.OpenSession(null, false, false, ConnectionReleaseMode.AfterStatement) :
																sessionImplementor.Factory.OpenSession(null, false, false, ConnectionReleaseMode.AfterStatement);
			try 
			{
				proxy.HibernateLazyInitializer.Session = (ISessionImplementor) tempSession;
				proxy.HibernateLazyInitializer.Initialize();
				return proxy.HibernateLazyInitializer.GetImplementation();
			} 
			finally 
			{
				tempSession.Close();
			}
		}

        public static bool ObjectsEqual(object obj1, object obj2)
        {
			if (obj1 == null) 
            {
				return obj2 == null;
			}

			return obj1.Equals(obj2);
		}


		public static bool IteratorsContentEqual(IEnumerator iter1, IEnumerator iter2)
		{
			while (iter1.MoveNext() && iter2.MoveNext()) 
            {
				if (!iter1.Current.Equals(iter2.Current)) 
                {
					return false;
				}
			}

			//noinspection RedundantIfStatement
			if (iter1.MoveNext() || iter2.MoveNext()) 
            {
				return false;
			}

			return true;
		}

		/**
		 * Transforms a list of arbitrary elements to a list of index-element pairs.
		 * @param list List to transform.
		 * @return A list of pairs: ((0, element_at_index_0), (1, element_at_index_1), ...)
		 */
		public static IList<Pair<int, T>> ListToIndexElementPairList<T>(IList list)
		{
			var ret = new List<Pair<int, T>>();

			for (var i = 0; i < list.Count; i++)
			{
				ret.Add(Pair<int,T>.Make(i, (T)list[i]));
			}

			return ret;
		}

		/**
		 * @param properties Properties from which to read.
		 * @param propertyName The name of the property.
		 * @param defaultValue Default value returned if no value for {@code propertyName} is set.
		 * @return The value of the property or the default value, if property is not set.
		 */
        public static string GetProperty(IDictionary<string, string> properties, string propertyName, string defaultValue)
		{
		    string ret;
		    return properties.TryGetValue(propertyName, out ret) ? ret : defaultValue;
		}

		public static System.Type ResolveDotnetType(string className)
	    {
	        return ReflectHelper.ClassForFullName(className);
		}
	}
}

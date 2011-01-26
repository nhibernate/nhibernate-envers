using System;
using System.Collections.Generic;
using NHibernate.Proxy;
using NHibernate.Engine;
using System.Collections;
using Iesi.Collections.Generic;
using System.Reflection;

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

		public static object GetIdentifier(ISessionImplementor session, object obj) 
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

		public static bool ObjectsEqual(Object obj1, Object obj2) {
			if (obj1 == null) {
				return obj2 == null;
			}

			return obj1.Equals(obj2);
		}


		public static bool IteratorsContentEqual(IEnumerator iter1, IEnumerator iter2)
		{
			while (iter1.MoveNext() && iter2.MoveNext()) {
				if (!iter1.Current.Equals(iter2.Current)) {
					return false;
				}
			}

			//noinspection RedundantIfStatement
			if (iter1.MoveNext() || iter2.MoveNext()) {
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
		public static String GetProperty(IDictionary<string, string> properties, String propertyName, String defaultValue)
		{
			if (properties.Keys.Contains(propertyName))
				return properties[propertyName];
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Type coresponding to the type name passed as parameter, looking in all assemblies
		/// of the app domain. Inspired from Drools.NET
		/// Throws an exception is more than one Type is found.
		/// </summary>
		/// <param name="className">the name of the type</param>
		/// <returns>the unique System.Type corresponding to the name passed</returns>
		public static System.Type ResolveDotnetType(string className)
		{
			System.Type clazz = null;
			AppDomain currentDomain = AppDomain.CurrentDomain;

			// try loading className
			if (clazz == null)
			{
				try {
					clazz = System.Type.GetType(className);
				}
				catch (System.Exception e) { clazz = null; }
			}

			if (clazz == null)
			{
				System.Collections.ArrayList validTypes = new System.Collections.ArrayList();
				//check is the type is in any referenced assemblies
				foreach (Assembly assembly in currentDomain.GetAssemblies())
				{
					clazz = assembly.GetType(className);
					if (clazz != null) validTypes.Add(clazz);
				}

				if (validTypes.Count > 1)
				{
					throw new System.Exception("Ambiguous class reference : " + className);
				}
				else if (validTypes.Count == 1)
				{
					clazz = (System.Type)validTypes[0];
				}
				else
				{
					clazz = null;
				}
			}
			return clazz;
		}
	}
}

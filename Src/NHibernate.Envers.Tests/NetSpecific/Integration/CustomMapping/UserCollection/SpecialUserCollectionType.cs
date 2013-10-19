using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialUserCollectionType : IUserCollectionType, IParameterizedType
	{
		private int limit;

		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new SpecialPersistentGenericBag(session, limit);
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new SpecialPersistentGenericBag(session, (ICollection<Number>)collection, limit);
		}

		public IEnumerable GetElements(object collection)
		{
			return (IEnumerable) collection;
		}

		public bool Contains(object collection, object entity)
		{
			throw new NotImplementedException();
		}

		public object IndexOf(object collection, object entity)
		{
			throw new NotImplementedException();
		}

		public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			var result = (ISpecialCollection)target;
			result.Clear();
			foreach (var o in (ISpecialCollection)original)
			{
				result.Add(o);
			}
			return result;
		}

		public object Instantiate(int anticipatedSize)
		{
			return new SpecialCollection(limit);
		}

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			limit = Convert.ToInt32(parameters["limit"]);
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.ImplicitEnversCollectionType
{
	public class ChildCollectionType : IUserCollectionType
	{
		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentGenericBag<Child>(session);
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentGenericBag<Child>(session, (IList<Child>)collection);
		}

		public IEnumerable GetElements(object collection)
		{
			return (IEnumerable)collection;
		}

		public bool Contains(object collection, object entity)
		{
			throw new System.NotImplementedException();
		}

		public object IndexOf(object collection, object entity)
		{
			throw new System.NotImplementedException();
		}

		public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			throw new System.NotImplementedException();
		}

		public object Instantiate(int anticipatedSize)
		{
			return new List<Child>();
		}
	}
}
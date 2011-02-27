using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class ListProxy<T> : CollectionProxy<T, IList<T>>, IList<T>
	{
		public ListProxy()
		{
		}

		public ListProxy(IInitializor<IList<T>> initializor)
					:base(initializor) 
		{
		}

		public int IndexOf(T item)
		{
			CheckInit();
			return CollectionDelegate.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			CheckInit();
			CollectionDelegate.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			CheckInit();
			CollectionDelegate.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				CheckInit();
				return CollectionDelegate[index];
			}
			set
			{
				CheckInit();
				CollectionDelegate[index] = value;
			}
		}
	}
}
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
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
			return delegat.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			CheckInit();
			delegat.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			CheckInit();
			delegat.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				CheckInit();
				return delegat[index];
			}
			set
			{
				CheckInit();
				delegat[index] = value;
			}
		}
	}
}
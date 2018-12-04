using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class ListProxy<T> : CollectionProxy<T>, IList<T>
	{
		public ListProxy(IInitializor initializor) : base(initializor)
		{
		}

		public int IndexOf(T item)
		{
			return GetCollection<IList<T>>().IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			GetCollection<IList<T>>().Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			GetCollection<IList<T>>().RemoveAt(index);
		}

		public T this[int index]
		{
			get => GetCollection<IList<T>>()[index];
			set => GetCollection<IList<T>>()[index] = value;
		}
	}
}
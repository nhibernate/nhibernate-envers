using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	public class ListProxy<T> : IList<T>
	{
		private readonly Lazy<IList<T>> _delegate;
		
		public ListProxy(IInitializor initializor)
		{
			_delegate = new Lazy<IList<T>>(() => (IList<T>) initializor.Initialize());
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _delegate.Value.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			_delegate.Value.Add(item);
		}

		public void Clear()
		{
			_delegate.Value.Clear();
		}

		public bool Contains(T item)
		{
			return _delegate.Value.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_delegate.Value.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return _delegate.Value.Remove(item);
		}

		public int Count => _delegate.Value.Count;

		public bool IsReadOnly => _delegate.Value.IsReadOnly;

		public int IndexOf(T item)
		{
			return _delegate.Value.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_delegate.Value.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_delegate.Value.RemoveAt(index);
		}

		public T this[int index]
		{
			get => _delegate.Value[index];
			set => _delegate.Value[index] = value;
		}
	}
}
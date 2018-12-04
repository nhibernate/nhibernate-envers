using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	public class SetProxy<T> : ISet<T>
	{
		private readonly Lazy<ISet<T>> _delegate;
		
		public SetProxy(IInitializor initializor)
		{
			_delegate = new Lazy<ISet<T>>(() => (ISet<T>) initializor.Initialize());
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _delegate.Value.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection<T>.Add(T item)
		{
			_delegate.Value.Add(item);
		}

		public void UnionWith(IEnumerable<T> other)
		{
			_delegate.Value.UnionWith(other);
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			_delegate.Value.IntersectWith(other);
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			_delegate.Value.ExceptWith(other);
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			_delegate.Value.SymmetricExceptWith(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return _delegate.Value.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return _delegate.Value.IsSupersetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return _delegate.Value.IsProperSupersetOf(other);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return _delegate.Value.IsProperSubsetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			return _delegate.Value.Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			return _delegate.Value.SetEquals(other);
		}

		bool ISet<T>.Add(T item)
		{
			return _delegate.Value.Add(item);
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
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class SetProxy<T> : ISet<T>
	{
		[NonSerialized]
		private readonly IInitializor _initializor;
		private ISet<T> _delegate;
		
		public SetProxy(IInitializor initializor)
		{
			_initializor = initializor;
		}
		
		private void checkInit()
		{
			if (_delegate == null)
			{
				_delegate = (ISet<T>) _initializor.Initialize();
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			checkInit();
			return _delegate.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			checkInit();
			return GetEnumerator();
		}

		void ICollection<T>.Add(T item)
		{
			checkInit();
			_delegate.Add(item);
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			checkInit();
			_delegate.ExceptWith(other);
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			checkInit();
			_delegate.IntersectWith(other);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			checkInit();
			return _delegate.IsProperSubsetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			checkInit();
			return _delegate.IsProperSupersetOf(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			checkInit();
			return _delegate.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			checkInit();
			return _delegate.IsSupersetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			checkInit();
			return _delegate.Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			checkInit();
			return _delegate.SetEquals(other);
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			checkInit();
			_delegate.SymmetricExceptWith(other);
		}

		public void UnionWith(IEnumerable<T> other)
		{
			checkInit();
			_delegate.UnionWith(other);
		}

		bool ISet<T>.Add(T item)
		{
			checkInit();
			return _delegate.Add(item);
		}

		public void Clear()
		{
			checkInit();
			_delegate.Clear();
		}

		public bool Contains(T item)
		{
			checkInit();
			return _delegate.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			checkInit();
			_delegate.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			checkInit();
			return _delegate.Remove(item);
		}

		public int Count
		{
			get
			{
				checkInit();
				return _delegate.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				checkInit();
				return _delegate.IsReadOnly;
			}
		}
	}
}
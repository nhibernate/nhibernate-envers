using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class ListProxy<T> : IList<T>
	{
		[NonSerialized]
		private readonly IInitializor _initializor;
		private IList<T> _delegate;
		
		public ListProxy(IInitializor initializor)
		{
			_initializor = initializor;
		}

		private void checkInit()
		{
			if (_delegate == null)
			{
				_delegate = (IList<T>) _initializor.Initialize();
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

		public void Add(T item)
		{
			checkInit();
			_delegate.Add(item);
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

		public int IndexOf(T item)
		{
			checkInit();
			return _delegate.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			checkInit();
			_delegate.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			checkInit();
			_delegate.RemoveAt(index);
		}

		public T this[int index]
		{
			get 
			{ 				
				checkInit();
				return _delegate[index]; 
			}
			set
			{
				checkInit();
				_delegate[index] = value;
			}
		}
	}
}
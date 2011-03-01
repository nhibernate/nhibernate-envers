using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public abstract class CollectionProxy<TItem, TCollection> : ICollection<TItem>
							where TCollection : class, ICollection<TItem>
	{
		[NonSerialized]
		private readonly IInitializor<TCollection> initializor;

		protected CollectionProxy() 
		{
		}

		protected CollectionProxy(IInitializor<TCollection> initializor) 
		{
			this.initializor = initializor;
		}

		protected TCollection CollectionDelegate { get; private set; }

		protected void CheckInit() 
		{
			if (CollectionDelegate == null)
			{
				CollectionDelegate = initializor.Initialize();
			}
		}

		public int Count 
		{
			get 
			{
				CheckInit();
				return CollectionDelegate.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				CheckInit();
				return CollectionDelegate.IsReadOnly;
			}
		}

		public bool Contains(TItem item) 
		{
			CheckInit();
			return CollectionDelegate.Contains(item);
		}

		public void CopyTo(TItem[] array, int arrayIndex)
		{
			CheckInit();
			CollectionDelegate.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TItem> GetEnumerator() 
		{
			CheckInit();
			return CollectionDelegate.GetEnumerator();
		}

		public void Add(TItem item) 
		{
			CheckInit();
			CollectionDelegate.Add(item);
		}

		public bool Remove(TItem item) 
		{
			CheckInit();
			return CollectionDelegate.Remove(item);
		}

		public void Clear() 
		{
			CheckInit();
			CollectionDelegate.Clear();
		}

		public override string ToString() 
		{
			CheckInit();
			return CollectionDelegate.ToString();
		}

		public override bool Equals(object obj) 
		{
			CheckInit();
			return CollectionDelegate.Equals(obj);
		}

		public override int GetHashCode() 
		{
			CheckInit();
			return CollectionDelegate.GetHashCode();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

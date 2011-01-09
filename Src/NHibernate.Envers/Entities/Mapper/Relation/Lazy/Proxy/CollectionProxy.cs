using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	public abstract class CollectionProxy<TItem, TCollection> : ICollection<TItem>
							where TCollection : class, ICollection<TItem>
	{
		private readonly IInitializor<TCollection> initializor;
		protected TCollection delegat;

		protected CollectionProxy() 
		{
		}

		protected CollectionProxy(IInitializor<TCollection> initializor) 
		{
			this.initializor = initializor;
		}

		protected void CheckInit() 
		{
			if (delegat == null)
			{
				delegat = initializor.Initialize();
			}
		}

		public int Count 
		{
			get 
			{
				CheckInit();
				return delegat.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				CheckInit();
				return delegat.IsReadOnly;
			}
		}

		public bool Contains(TItem o) 
		{
			CheckInit();
			return delegat.Contains(o);
		}

		public void CopyTo(TItem[] array, int arrayIndex)
		{
			CheckInit();
			delegat.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TItem> GetEnumerator() 
		{
			CheckInit();
			return delegat.GetEnumerator();
		}

		public void Add(TItem o) 
		{
			CheckInit();
			delegat.Add(o);
		}

		public bool Remove(TItem o) 
		{
			CheckInit();
			return delegat.Remove(o);
		}

		public void Clear() 
		{
			CheckInit();
			delegat.Clear();
		}

		public override string ToString() 
		{
			CheckInit();
			return delegat.ToString();
		}

		public override bool Equals(object obj) 
		{
			CheckInit();
			return delegat.Equals(obj);
		}

		public override int GetHashCode() 
		{
			CheckInit();
			return delegat.GetHashCode();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
